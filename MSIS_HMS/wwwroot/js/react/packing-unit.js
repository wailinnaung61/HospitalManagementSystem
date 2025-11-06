const { Component, useState, useEffect, useRef } = React;

$(document).ready(function () {
    $('#BranchId').change(function (e) {
        window.reactComponent.setState({ branchId: e.target.value });
    });
});

const PackingUnitForm = (props) => {
    const { branchId } = props;
    const [itemId, setItemId] = useState(_id);
    const [action, setAction] = useState(_action);
    const [unitPrice, setUnitPrice] = useState(_unitPrice);
    const [percentageForSale, setPercentageForSale] = useState(_percentageForSale);
    const [percentageForDiscount, setPercentageForDiscount] = useState(_percentageForDiscount);
    const [units, setUnits] = useState([]);
    const [selectUnitId, setSelectUnitId] = useState('');
    const [unitLevel, setUnitLevel] = useState(0);
    const [packingUnits, setPackingUnits] = useState(_packingUnits);

    useEffect(() => {
        //$('#BranchId').trigger('change');
    }, [])

    useEffect(() => {
        console.log(packingUnits)
    }, [packingUnits])

    useEffect(() => {
        $.ajax({
            url: `/units/getall`,
            type: 'get',
            success: function (res) {
                var units = res.filter(x => x.unitLevel > unitLevel);
                setUnits(JSON.parse(JSON.stringify(units || [])))
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        })

        return () => {
            setUnits([]);
            setSelectUnitId('');
            setUnitLevel(0);
            setPackingUnits([]);
        }
    }, []);

    useEffect(() => {
        setSelectUnitId('');
        var units_desc = (sort(packingUnits, "unitLevel")).reverse();
        setUnitLevel(units_desc.length > 0 ? units_desc[0].unitLevel : 0);
    }, [packingUnits]);

    useDidUpdateEffect(() => {
        calculateAmount(packingUnits, unitPrice, percentageForSale, percentageForDiscount);
    }, [unitPrice, percentageForSale, percentageForDiscount])

    const validateStr = str => str != null && str != undefined && str != "";

    const calculateAmount = (packingUnits, unitPrice, percentageForSale, percentageForDiscount) => {
        unitPrice = validateStr(unitPrice) ? parseFloat(unitPrice) : 0;
        percentageForSale = validateStr(percentageForSale) ? parseFloat(percentageForSale) : 0;
        if (packingUnits.length > 0) {
            for (var i = 0; i < packingUnits.length; i++) {
                var purchaseAmount = 0, saleAmount = 0;
                var smallestQty = calculateQtyInSmallestUnit(packingUnits, packingUnits[i].unitLevel);
                purchaseAmount = parseFloat((unitPrice / smallestQty).toFixed(2));
                saleAmount = parseFloat((purchaseAmount + (purchaseAmount * (percentageForSale / 100))).toFixed(2));
                packingUnits[i].purchaseAmount = purchaseAmount;
                packingUnits[i].saleAmount = (saleAmount - (saleAmount * (percentageForDiscount / 100))).toFixed(2);
            }
            setPackingUnits(JSON.parse(JSON.stringify(packingUnits)));
        }
    }

    const calculateQtyInSmallestUnit = (packingUnits, smallestUnitLevel) => {
        debugger;
        var qty = 0;
        packingUnits = (sort(packingUnits, "unitLevel")).filter(x => x.unitLevel <= smallestUnitLevel);
        for (var i = 0; i < packingUnits.length; i++) {
            if (i == 0) {
                qty = packingUnits[i].qtyInParent;
            } else {
                qty *= packingUnits[i].qtyInParent;
            }
        }
        return qty;
    }

    const onChangeUnit = (e) => {
        setSelectUnitId(e.target.value);
    }

    const onAddUnit = () => {
        var unit = units.find(x => x.id == selectUnitId);
        if (unit) {
            var packingUnit = {
                itemId: itemId || 0,
                unitId: unit.id,
                unitName: unit.name,
                unitLevel: unit.unitLevel,
                qtyInParent: 1,
                purchaseAmount: 0,
                saleAmount: 0,
            };
            packingUnits.push(packingUnit);
            setPackingUnits(JSON.parse(JSON.stringify(packingUnits)));
        }
    }

    const onDeleteUnit = (id) => {
        var _packingUnits = arrayRemoveByKey(packingUnits, "unitId", id);
        setPackingUnits(JSON.parse(JSON.stringify(_packingUnits)));
    }

    return (
        <div className="col-md-12 form-group p-50">
            <div className="col-md-12">
                <h2 className="form-title pull-left">Packing Units</h2>
                <div className="actions panel_actions pull-right">
                </div>
            </div>
            <div className="col-md-3 form-group">
                <label className="form-label" for="Description">Unit Price</label>
                <span className="desc"></span>
                <div className="controls">
                    <input type="number" onChange={(e) => setUnitPrice(e.target.value)} value={unitPrice} className="form-control" id="UnitPrice" name="UnitPrice" />
                    <span asp-validation-for="Description" className="text-danger"></span>
                </div>
            </div>
            <div className="col-md-3 form-group">
                <label className="form-label" for="PercentageForSale">Percentage For Sale</label>
                <span className="desc"></span>
                <div className="controls">
                    <input type="number" onChange={(e) => setPercentageForSale(e.target.value)} value={percentageForSale} className="form-control" id="PercentageForSale" name="PercentageForSale" />
                    <span asp-validation-for="PercentageForSale" className="text-danger"></span>
                </div>
            </div>
            <div className="col-md-3 form-group">
                <label className="form-label" for="PercentageForDiscount">Discount (%)</label>
                <span className="desc"></span>
                <div className="controls">
                    <input type="number" onChange={(e) => setPercentageForDiscount(e.target.value)} value={percentageForDiscount} className="form-control" id="PercentageForDiscount" name="PercentageForDiscount" />
                    <span asp-validation-for="PercentageForDiscount" className="text-danger"></span>
                </div>
            </div>
            <div className="col-md-12 form-group">
                {
                    action == "Edit" ? <div></div> :
                        <div className="row">
                            <div className="col-md-3 form-group">
                                <label className="form-label" for="Description">Unit</label>
                                <span className="desc"></span>
                                <div className="controls">
                                    <select class="form-control" onChange={onChangeUnit} value={selectUnitId} asp-for="BranchId" asp-items="ViewBag.Branches">
                                        <option value="">Please Select Unit</option>
                                        {
                                            units.filter(x => x.unitLevel > unitLevel).map((unit, index) =>
                                                <option key={index} value={unit.id}>{unit.name}</option>
                                            )
                                        }
                                    </select>
                                </div>
                            </div>
                            <div className="col-md-3 form-group">
                                <label className="form-label">&nbsp;</label>
                                <span className="desc"></span>
                                <div className="controls">
                                    <button type="button" onClick={onAddUnit} class="btn btn-primary"><i class="fa fa-plus"></i></button>
                                </div>
                            </div>
                        </div>
                }
                <table id="example" class="display table table-hover table-condensed">
                    <thead>
                        <tr>
                            <th>Level</th>
                            <th>Unit</th>
                            <th>Qty In Parent</th>
                            <th>Purchase Price</th>
                            <th>Sale Price</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            sort(packingUnits, "unitLevel").map((packingUnit, index) => {
                                return (
                                    <tr key={index}>
                                        <td>{packingUnit.unitLevel}</td>
                                        <td>
                                            <input hidden name={`PackingUnits[${index}].ItemId`} value={packingUnit.itemId} />
                                            <input hidden name={`PackingUnits[${index}].UnitId`} value={packingUnit.unitId} />
                                            <input hidden name={`PackingUnits[${index}].UnitName`} value={packingUnit.unitName} />
                                            <input hidden name={`PackingUnits[${index}].UnitLevel`} value={packingUnit.unitLevel} />
                                            {packingUnit.unitName}
                                        </td>
                                        <td><input type="number" className="form-control disabled" name={`PackingUnits[${index}].QtyInParent`} onChange={async (e) => { packingUnit.qtyInParent = e.target.value; await setPackingUnits(JSON.parse(JSON.stringify(packingUnits))); calculateAmount(packingUnits, unitPrice, percentageForSale); }} value={packingUnit.qtyInParent} /></td>
                                        <td><input type="number" className="form-control" name={`PackingUnits[${index}].PurchaseAmount`} onChange={(e) => { packingUnit.purchaseAmount = e.target.value; setPackingUnits(JSON.parse(JSON.stringify(packingUnits))); }} value={packingUnit.purchaseAmount} /></td>
                                        <td><input type="number" className="form-control" name={`PackingUnits[${index}].SaleAmount`} onChange={(e) => { packingUnit.saleAmount = e.target.value; setPackingUnits(JSON.parse(JSON.stringify(packingUnits))); }} value={packingUnit.saleAmount} /></td>
                                        <td>
                                            {
                                                action == "Edit" ? <span></span> :
                                                    <button type="button" onClick={() => onDeleteUnit(packingUnit.unitId)} class="btn btn-xs btn-secondary">
                                                        Delete
                                                    </button>
                                            }
                                        </td>
                                    </tr>
                                )
                            })
                        }
                    </tbody>
                </table>
            </div>
        </div>
    );
}

class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            branchId: undefined,
        }
    }

    componentWillUnmount() {
        this.setState({ branchId: undefined });
    }

    render() {
        return (
            <PackingUnitForm {...this.state} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)