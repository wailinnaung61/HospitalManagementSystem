const { Component, useState, useEffect, useRef, Fragment } = React;
$(document).ready(function () {

});
const ScratchItemForm = (props) => {
    const { warehouseId, selectedItemId, selectedUnitId, selectedBatchId } = props;
    const [qty, setQty] = useState('');
    const [items, setItems] = useState([]);
    const [packingUnits, setPackingUnits] = useState([]);
    const [batches, setBatches] = useState([]);
    const [scratchItems, setScratchItems] = useState(_scratchItems);
    const [error, setError] = useState(_error);

    useEffect(() => {
        $('#ItemId').select2();
        $('#UnitId').select2();
        $('#BatchId').select2();
        $('#ItemId').change(function (e) {
            window.reactComponent.setState({ selectedItemId: e.target.value });
        });
        $('#UnitId').change(function (e) {
            window.reactComponent.setState({ selectedUnitId: e.target.value });
        });
        $('#BatchId').change(function (e) {
            window.reactComponent.setState({ selectedBatchId: e.target.value });
        });
        $('.warehouse').change(function (e) {
            window.reactComponent.setState({ warehouseId: e.target.value });

        });
    }, []);

    useEffect(() => {
        //console.log("warehouseId change");
        if (warehouseId) {
            $.ajax({
                url: `/warehouses/GetWarehouseItems?warehouseId=${warehouseId}`,
                type: 'get',
                success: function (res) {
                    setItems(JSON.parse(JSON.stringify(res || [])));
                },
                error: function (jqXhr, textStatus, errorMessage) {

                }
            });
        }

        return () => {
            setQty('');
            setItems([]);
            setPackingUnits([]);
            setBatches([]);
            setScratchItems([]);
            setError('');
        }
    }, [warehouseId]);

    useDidUpdateEffect(() => {
        var selectedItem = items.find(x => x.id == selectedItemId);
        if (selectedItem) {
            setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
            $.ajax({
                url: `/batches/getall?ItemId=${selectedItemId}`,
                type: 'get',
                success: function (res) {
                    setBatches(JSON.parse(JSON.stringify(res || [])));
                },
                error: function (jqXhr, textStatus, errorMessage) {

                }
            });
        } else {
            $('#ItemId').val('').trigger('change');
            setPackingUnits(JSON.parse(JSON.stringify([])));
            setBatches(JSON.parse(JSON.stringify([])));
        }
    }, [selectedItemId]);


    const onAddItem = () => {
        if (selectedItemId && selectedUnitId && selectedBatchId && !isEmpty(qty) ) {
            if (qty <= 0) {
                setError('Qty must be greater than 0');
            }else {
                setError('');
                var selectedItem = items.find(x => x.id == selectedItemId);
                var selectedUnit = packingUnits.find(x => x.unitId == selectedUnitId);
                var selectedBatch = batches.find(x => x.id == selectedBatchId);
                var scratchItem = {
                    itemId: selectedItemId,
                    unitId: selectedUnitId,
                    batchId: selectedBatchId,
                    qty: Number(qty),
                    qtyInSmallestUnit: Number(calculateQtyInSmallestUnit(packingUnits, selectedUnitId, qty)),
                   
                   
                    itemName: selectedItem ? selectedItem.name : '',
                    unitName: selectedUnit ? selectedUnit.unitName : '',
                    batchName: selectedBatch ? selectedBatch.name : '',
                };
                scratchItems.push(scratchItem);
                setScratchItems(JSON.parse(JSON.stringify(scratchItems)));
            }
        } else {
            setError('* fields are required');
        }
    }

    useEffect(() => {
        console.log('Deliver Order Items', scratchItems);
        props.reset();
        setQty('');
    }, [scratchItems])

    const onDeleteItem = (index) => {
        var packingUnits = arrayRemoveByIndex(scratchItems, index);
        setPackingUnits(JSON.parse(JSON.stringify(packingUnits)));
    }

    return (
        <div className="col-md-12 form-group p-50">
            <div className="col-md-12">
                <h2 className="form-title pull-left">Scratch Items</h2>
                <div className="actions panel_actions pull-right">
                </div>
            </div>
            <div className="col-md-12">
                <div className="row">
                    <div className="col-md-4 form-group">
                        <label className="form-label" for="ItemId">Item</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <select className="form-control" id="ItemId">
                                <option value="">Please Select Item</option>
                                {
                                    items.map((item, index) =>
                                        <option key={index} value={item.id}>{item.name} ({item.code})</option>
                                    )
                                }
                            </select>
                        </div>
                    </div>
                    <div className="col-md-4 form-group">
                        <label className="form-label" for="UnitId">Unit</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <select className="form-control" id="UnitId">
                                <option value="">Please Select Unit</option>
                                {
                                    packingUnits.map((packingUnit, index) =>
                                        <option key={index} value={packingUnit.unitId}>{packingUnit.unitName}</option>
                                    )
                                }
                            </select>
                        </div>
                    </div>
                    <div className="col-md-4 form-group">
                        <label className="form-label" for="BatchId">Batch</label>
                        <span className="desc"></span>
                        <div className="controls">
                            <select className="form-control" id="BatchId">
                                <option value="">Please Select Batch</option>
                                {
                                    batches.map((batch, index) =>
                                        <option key={index} value={batch.id}>{batch.name}</option>
                                    )
                                }
                            </select>
                        </div>
                    </div>
                </div>
                <div className="row">
                    <div className="col-md-3 form-group">
                        <label className="form-label" for="Qty">Qty</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <input type="number" id="Qty" className="form-control" onChange={(e) => setQty(e.target.value)} value={qty} min={0} />
                        </div>
                    </div>
                    <div className="col-md-2 form-group">
                        <label className="form-label">&nbsp;</label>
                        <span className="desc"></span>
                        <div className="controls">
                            <button type="button" onClick={onAddItem} className="btn btn-primary"><i className="fa fa-plus"></i></button>
                        </div>
                    </div>
                                 
                </div>
               
            </div>
            <div className="col-md-12 form-group">
                {
                    error && <span className="text-danger mb-5">{error}</span>
                }
                <table id="example" className="display table table-hover table-condensed">
                    <thead>
                        <tr>
                            <th>No.</th>
                            <th>Item</th>
                            <th>Unit</th>
                            <th>Batch</th>
                            <th>Qty</th>                           
                            <th style={{ textAlign: 'center' }}></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            sort(scratchItems, "sortOrder").map((purchaseItem, index) => {
                                return (
                                    <tr key={index}>
                                        <td>
                                            <input name={`ScratchItems[${index}].SortOrder`} value={index + 1} hidden />
                                            {index + 1}
                                        </td>
                                        <td>
                                            <input name={`ScratchItems[${index}].ItemId`} value={purchaseItem.itemId} hidden />
                                            <input name={`ScratchItems[${index}].ItemName`} value={purchaseItem.itemName} hidden />
                                            {purchaseItem.itemName || (purchaseItem.item ? purchaseItem.item.name : '')}
                                        </td>
                                        <td>
                                            <input name={`ScratchItems[${index}].UnitId`} value={purchaseItem.unitId} hidden />
                                            <input name={`ScratchItems[${index}].UnitName`} value={purchaseItem.unitName} hidden />
                                            {purchaseItem.unitName || (purchaseItem.unit ? purchaseItem.unit.name : '')}
                                        </td>
                                        <td>
                                            <input name={`ScratchItems[${index}].BatchId`} value={purchaseItem.batchId} hidden />
                                            <input name={`ScratchItems[${index}].BatchName`} value={purchaseItem.batchName} hidden />
                                            {purchaseItem.batchName || (purchaseItem.batch ? purchaseItem.batch.name : '')}
                                        </td>
                                        <td>
                                            <input name={`ScratchItems[${index}].Qty`} value={purchaseItem.qty} hidden />
                                            <input name={`ScratchItems[${index}].QtyInSmallestUnit`} value={purchaseItem.qtyInSmallestUnit} hidden />
                                            {purchaseItem.qty}
                                        </td>
                                       
                                        <td>
                                            <button type="button" onClick={() => onDeleteItem(index)} className="btn btn-xs btn-secondary">Delete</button>
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
            warehouseId: _warehouseId && _warehouseId != 0 ? _warehouseId : undefined,
            selectedItemId: undefined,
            selectedUnitId: undefined,
            selectedBatchId: undefined,
        }

        this.clear = this.clear.bind(this);
    }

    componentWillUnmount() {
        this.clear();
    }

    clear = () => this.setState({ selectedItemId: undefined, selectedUnitId: undefined, selectedBatchId: undefined });

    render() {
        return (
            <ScratchItemForm {...this.state} reset={this.clear} />
        );
    }
}
const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)