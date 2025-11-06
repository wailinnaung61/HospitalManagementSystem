const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const PurchaseItemForm = (props) => {
    const { selectedItemId, selectedUnitId, selectedBatchId } = props;
    const [qty, setQty] = useState('');
    const [unitPrice, setUnitPrice] = useState('');
    const [isGiftItem, setIsGiftItem] = useState(false);
    const [items, setItems] = useState([]);
    const [packingUnits, setPackingUnits] = useState([]);
    const [batches, setBatches] = useState([]);
    const [purchaseItems, setPurchaseItems] = useState(_purchaseItems);
    const [total, setTotal] = useState(0);
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
    }, []);

    useEffect(() => {
        $.ajax({
            url: `/items/getall`,
            type: 'get',
            success: function (res) {
                setItems(JSON.parse(JSON.stringify(res || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });

        return () => {
            setQty('');
            setUnitPrice('');
            setIsGiftItem(false);
            setItems([]);
            setPackingUnits([]);
            setBatches([]);
            setPurchaseItems([]);
            setError('');
        }
    }, []);

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

    useEffect(() => {
        if (isGiftItem) {
            setUnitPrice(0);
        }
    }, [isGiftItem]);

    useEffect(() => {
        setTotal(calculateTotal(purchaseItems.map(x => ({ amount: x.qty * x.unitPrice })), "amount"));
    }, [purchaseItems]);

    const onAddItem = () => {
        if (selectedItemId && selectedUnitId && selectedBatchId && !isEmpty(qty) && !isEmpty(unitPrice)) {
            if (qty <= 0) {
                setError('Qty must be greater than 0');
            } else if (!isGiftItem && unitPrice <= 0) {
                setError('Item must be gift item or Unit Price must be greater than 0');
            } else {
                setError('');
                var selectedItem = items.find(x => x.id == selectedItemId);
                var selectedUnit = packingUnits.find(x => x.unitId == selectedUnitId);
                var selectedBatch = batches.find(x => x.id == selectedBatchId);
                var purchaseItem = {
                    itemId: selectedItemId,
                    unitId: selectedUnitId,
                    batchId: selectedBatchId,
                    qty: Number(qty),
                    qtyInSmallestUnit: Number(calculateQtyInSmallestUnit(packingUnits, selectedUnitId, qty)),
                    unitPrice: Number(unitPrice),
                    isGiftItem,
                    itemName: selectedItem ? selectedItem.name : '',
                    unitName: selectedUnit ? selectedUnit.unitName : '',
                    batchName: selectedBatch ? selectedBatch.name : '',
                };
                purchaseItems.push(purchaseItem);
                setPurchaseItems(JSON.parse(JSON.stringify(purchaseItems)));
            }
        } else {
            setError('* fields are required');
        }
    }

    useEffect(() => {
        console.log('Purchase Items', purchaseItems);
        props.reset();
        setQty('');
        setUnitPrice('');
        setIsGiftItem(false);
    }, [purchaseItems])

    const onDeleteItem = (index) => {
        arrayRemoveByIndex(purchaseItems, index);
        setPurchaseItems(JSON.parse(JSON.stringify(purchaseItems)));
    }

    return (
        <div className="col-md-12 form-group p-50">
            <div className="col-md-12">
                <h2 className="form-title pull-left">Purchased Items</h2>
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
                            <input type="number" id="Qty" className="form-control" onChange={(e) => setQty(e.target.value)} value={qty} />
                        </div>
                    </div>
                    <div className="col-md-1 form-group">
                        <label className="form-label" for="IsGiftItem">{_itemType == "PurchaseItems" ? "Gift" : "FOC"}</label>
                        <span className="desc"></span>
                        <div className="controls">
                            <div className="form-group">
                                <input type="checkbox" onClick={(e) => setIsGiftItem(e.target.checked)} value={isGiftItem} checked={isGiftItem} className="skin-square-grey" id="IsGiftItem" />
                                {
                                    //<label className="icheck-label form-label" for="IsGiftItem">Gift Item</label>
                                }
                            </div>
                        </div>
                    </div>
                    <div className="col-md-3 form-group">
                        <label className="form-label" for="UnitPrice">Unit Price</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <input type="number" disabled={isGiftItem} id="UnitPrice" className="form-control" onChange={(e) => setUnitPrice(e.target.value)} value={unitPrice} />
                        </div>
                    </div>
                    <div className="col-md-3 form-group">
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
                            <th className="money">Unit Price</th>
                            <th className="money">Amount</th>
                            <th style={{ textAlign: 'center' }}></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            sort(purchaseItems, "sortOrder").map((purchaseItem, index) => {
                                return (
                                    <tr key={index}>
                                        <td>
                                            <input name={`${_itemType}[${index}].SortOrder`} value={index + 1} hidden />
                                            {index + 1}
                                        </td>
                                        <td>
                                            <input name={`${_itemType}[${index}].ItemId`} value={purchaseItem.itemId} hidden />
                                            <input name={`${_itemType}[${index}].ItemName`} value={purchaseItem.itemName} hidden />
                                            {purchaseItem.itemName || (purchaseItem.item ? purchaseItem.item.name : '')}
                                        </td>
                                        <td>
                                            <input name={`${_itemType}[${index}].UnitId`} value={purchaseItem.unitId} hidden />
                                            <input name={`${_itemType}[${index}].UnitName`} value={purchaseItem.unitName} hidden />
                                            {purchaseItem.unitName || (purchaseItem.unit ? purchaseItem.unit.name : '')}
                                        </td>
                                        <td>
                                            <input name={`${_itemType}[${index}].BatchId`} value={purchaseItem.batchId} hidden />
                                            <input name={`${_itemType}[${index}].BatchName`} value={purchaseItem.batchName} hidden />
                                            {purchaseItem.batchName || (purchaseItem.batch ? purchaseItem.batch.name : '')}
                                        </td>
                                        <td>
                                            <input name={`${_itemType}[${index}].Qty`} value={purchaseItem.qty} hidden />
                                            <input name={`${_itemType}[${index}].QtyInSmallestUnit`} value={purchaseItem.qtyInSmallestUnit} hidden />
                                            {purchaseItem.qty}
                                        </td>
                                        <td className="money">
                                            <input name={`${_itemType}[${index}].IsGiftItem`} value={purchaseItem.isGiftItem} hidden />
                                            <input name={`${_itemType}[${index}].UnitPrice`} value={purchaseItem.unitPrice} hidden />
                                            {purchaseItem.isGiftItem ? 'Gift Item' : purchaseItem.unitPrice.formatMoney()}
                                        </td>
                                        <td className="money">{(purchaseItem.qty * purchaseItem.unitPrice).formatMoney()}</td>
                                        <td>
                                            <button type="button" onClick={() => onDeleteItem(index)} className="btn btn-xs btn-secondary">Delete</button>
                                        </td>
                                    </tr>
                                )
                            })
                        }
                    </tbody>
                    <tfoot>
                        <tr>
                            <td colSpan={6} className="money">Total</td>
                            <td className="money">{total.formatMoney()}</td>
                            <td></td>
                        </tr>
                    </tfoot>
                </table>
            </div>
        </div>
    );
}

class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
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
            <PurchaseItemForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)