const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const PurchaseItemForm = (props) => {
    const { warehouseId, selectedItemId, selectedUnitId, selectedBatchId } = props;
    const [qty, setQty] = useState('');
    const [tax,setTax] = useState(_tax);
    const [discount,setDiscount] = useState(_discount);
    const [unitPrice, setUnitPrice] = useState('');
    const [isFocItem, setIsFocItem] = useState(false);
    const [items, setItems] = useState([]);
    const [packingUnits, setPackingUnits] = useState([]);
    const [batches, setBatches] = useState([]);
    const [deliverOrderItems, setDeliverOrderItems] = useState(_deliverOrderItems);
    const [total, setTotal] = useState(0);
    const [bulkTotal,setBulkTotal] = useState(0);
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
            setTax(0);
            setDiscount(0);
            setUnitPrice('');
            setIsFocItem(false);
            setItems([]);
            setPackingUnits([]);
            setBatches([]);
            setDeliverOrderItems([]);
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

    useEffect(() => {
        if (isFocItem) {
            setUnitPrice(0);
        }
    }, [isFocItem]);

    useEffect(() => {
        setTotal(calculateTotal(deliverOrderItems.map(x => ({ amount: x.qty * x.unitPrice})), "amount"));
    }, [deliverOrderItems]);

    useEffect(() => {
        setBulkTotal(calculateBulkTotal(deliverOrderItems.map(x => ({ amount: x.qty * x.unitPrice})), "amount",Number(tax),Number(discount)));
    }, [deliverOrderItems]);
    

    const onAddItem = () => {
        if (selectedItemId && selectedUnitId && selectedBatchId && !isEmpty(qty) && !isEmpty(unitPrice)) {
            if (qty <= 0) {
                setError('Qty must be greater than 0');
            } else if (!isFocItem && unitPrice <= 0) {
                setError('Item must be foc item or Unit Price must be greater than 0');
            } else {
                setError('');
                var selectedItem = items.find(x => x.id == selectedItemId);
                var selectedUnit = packingUnits.find(x => x.unitId == selectedUnitId);
                var selectedBatch = batches.find(x => x.id == selectedBatchId);
                var deliverItem = {
                    itemId: selectedItemId,
                    unitId: selectedUnitId,
                    batchId: selectedBatchId,
                    qty: Number(qty),
                    qtyInSmallestUnit: Number(calculateQtyInSmallestUnit(packingUnits, selectedUnitId, qty)),
                    unitPrice: Number(unitPrice),
                    isFocItem,
                    itemName: selectedItem ? selectedItem.name : '',
                    unitName: selectedUnit ? selectedUnit.unitName : '',
                    batchName: selectedBatch ? selectedBatch.name : '',
                };
                deliverOrderItems.push(deliverItem);
                setDeliverOrderItems(JSON.parse(JSON.stringify(deliverOrderItems)));
            }
        } else {
            setError('* fields are required');
        }
    }

    useEffect(() => {
        console.log('Deliver Order Items', deliverOrderItems);
        props.reset();
        setQty('');
        //setTax(0);
        //setDiscount(0);
        setUnitPrice('');
        setIsFocItem(false);
    }, [deliverOrderItems])
    
    const onDeleteItem = (index) => {
        var packingUnits = arrayRemoveByIndex(deliverOrderItems, index);
        setPackingUnits(JSON.parse(JSON.stringify(packingUnits)));
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
                            <input type="number" id="Qty" className="form-control" onChange={(e) => setQty(e.target.value)} value={qty} min={0}/>
                        </div>
                    </div>
                    <div className="col-md-1 form-group">
                        <label className="form-label" for="IsFocItem">FOC</label>
                        <span className="desc"></span>
                        <div className="controls">
                            <div className="form-group">
                                <input type="checkbox" onClick={(e) => setIsFocItem(e.target.checked)} value={isFocItem} checked={isFocItem} className="skin-square-grey" id="IsFocItem" />
                                {
                                    //<label className="icheck-label form-label" for="IsFocItem">Foc Item</label>
                                }
                            </div>
                        </div>
                    </div>
                    <div className="col-md-3 form-group">
                        <label className="form-label" for="UnitPrice">Unit Price</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <input type="number" disabled={isFocItem} id="UnitPrice" className="form-control" onChange={(e) => setUnitPrice(e.target.value)} value={unitPrice} />
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
                        <th className="totalmoney" style={{ width: '200px' }}>Amount</th>
                        <th style={{ textAlign: 'center' }}></th>
                    </tr>
                    </thead>
                    <tbody>
                    {
                        sort(deliverOrderItems, "sortOrder").map((purchaseItem, index) => {
                            return (
                                <tr key={index}>
                                    <td>
                                        <input name={`DeliverOrderItems[${index}].SortOrder`} value={index + 1} hidden />
                                        {index + 1}
                                    </td>
                                    <td>
                                        <input name={`DeliverOrderItems[${index}].ItemId`} value={purchaseItem.itemId} hidden />
                                        <input name={`DeliverOrderItems[${index}].ItemName`} value={purchaseItem.itemName} hidden />
                                        {purchaseItem.itemName || (purchaseItem.item ? purchaseItem.item.name : '')}
                                    </td>
                                    <td>
                                        <input name={`DeliverOrderItems[${index}].UnitId`} value={purchaseItem.unitId} hidden />
                                        <input name={`DeliverOrderItems[${index}].UnitName`} value={purchaseItem.unitName} hidden />
                                        {purchaseItem.unitName || (purchaseItem.unit ? purchaseItem.unit.name : '')}
                                    </td>
                                    <td>
                                        <input name={`DeliverOrderItems[${index}].BatchId`} value={purchaseItem.batchId} hidden />
                                        <input name={`DeliverOrderItems[${index}].BatchName`} value={purchaseItem.batchName} hidden />
                                        {purchaseItem.batchName || (purchaseItem.batch ? purchaseItem.batch.name : '')}
                                    </td>
                                    <td>
                                        <input name={`DeliverOrderItems[${index}].Qty`} value={purchaseItem.qty} hidden />
                                        <input name={`DeliverOrderItems[${index}].QtyInSmallestUnit`} value={purchaseItem.qtyInSmallestUnit} hidden />
                                        {purchaseItem.qty}
                                    </td>
                                    <td className="money">
                                        <input name={`DeliverOrderItems[${index}].IsFocItem`} value={purchaseItem.isFocItem} hidden />
                                        <input name={`DeliverOrderItems[${index}].UnitPrice`} value={purchaseItem.unitPrice} hidden />
                                        {purchaseItem.isFocItem ? 'Foc Item' : purchaseItem.unitPrice.formatMoney()}
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
                        <td colSpan={6} className="money">Sub Total</td>
                        <td className="money">{total.formatMoney()}</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={6} className="money pt-td">Tax</td>
                        <td><input type="number" name={`Tax`} className="form-control money-input" onChange={(e) => setTax(Number(e.target.value))} value={tax} /></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={6} className="money pt-td">Discount</td>
                        <td><input type="number" name={`Discount`} className="form-control money-input" onChange={(e) => setDiscount(Number(e.target.value))} value={discount} /></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={6} className="money">Grand Total</td>
                        <td className="money">{(Number(total) + Number(tax) - Number(discount)).formatMoney()}</td>
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
            <PurchaseItemForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)