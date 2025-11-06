const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const TransferItemForm = (props) => {
    const { warehouseId, outletId, selectedItemId, selectedUnitId, selectedBatchId } = props;
    const [qty, setQty] = useState('');
    const [unitPrice, setUnitPrice] = useState('');
    const [isGiftItem, setIsGiftItem] = useState(false);
    const [items, setItems] = useState([]);
    const [packingUnits, setPackingUnits] = useState([]);
    const [batches, setBatches] = useState([]);
    const [transferItems, setTransferItems] = useState(_purchaseItems);
    const [error, setError] = useState(_error);
    const [transferType, setTransferType] = useState(_transferItemType);

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
        $('.outlet').change(function (e) {
            window.reactComponent.setState({ outletId: e.target.value });
        });
    }, []);

    useEffect(() => {
        /*console.log("warehoues Id change");*/
        if (warehouseId) {
            $.ajax({
                url: `/warehouses/GetWarehouseItems?warehouseId=${warehouseId}`,
                type: 'get',
                success: function (res) {
                    console.log("warehoues Item ", res);
                    setItems(JSON.parse(JSON.stringify(res || [])));
                },
                error: function (jqXhr, textStatus, errorMessage) {

                }
            });
        }
       
       
        return () => {
            setQty('');
            setUnitPrice('');
            setIsGiftItem(false);
            setItems([]);
            setPackingUnits([]);
            setBatches([]);
            setTransferItems([]);
            setError('');
        }
    }, [warehouseId]);

   

    useEffect(() => {
    /*    console.log("Outlet Id change");*/
        if (outletId) {
            $.ajax({
                url: `/outlets/GetOutletItems?outletId=${outletId}`,
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
            setUnitPrice('');
            setIsGiftItem(false);
            setItems([]);
            setBatches([]);
            setPackingUnits([]);
            setTransferItems([]);
            setError('');
        }
    }, [outletId]);

    useEffect(() => {
        //console.log("Items change");
        $('#ItemId').val('').trigger('change');
        setPackingUnits(JSON.parse(JSON.stringify([])));
        setBatches(JSON.parse(JSON.stringify([])));
    }, [items]);

    useDidUpdateEffect(() => {
        //console.log("selectedItemId change");
        var selectedItem = items.find(x => x.id == selectedItemId);
        if (transferType == "ReturnItems") {
            if (selectedItem) {
                setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
                $.ajax({
                    url: `/Batches/GetAll?ItemId=${selectedItemId}`,
                    type: 'get',
                    success: function (res) {
                        setBatches(JSON.parse(JSON.stringify(res || [])));
                    },
                    error: function (jqXhr, textStatus, errorMessage) {

                    }
                });
            }
            else {
                $('#ItemId').val('').trigger('change');
                setPackingUnits(JSON.parse(JSON.stringify([])));
                setBatches(JSON.parse(JSON.stringify([])));
            }
        }
        else {
            if (selectedItem) {
                setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
                $.ajax({
                    url: `/warehouses/GetBatchesOfWarehouseItem?WarehouseId=${warehouseId}&ItemId=${selectedItemId}`,
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
        }
       
    }, [selectedItemId]);

    const onAddItem = () => {
        //console.log("onAddItem");
        var realQty = 0;
        debugger;
        if (selectedItemId && selectedUnitId && selectedBatchId && !isEmpty(qty)) {
            $.ajax({
                url: `/warehouses/GetQtyOfWarehouseItem?WarehouseId=${warehouseId}&ItemId=${selectedItemId}&BatchId=${selectedBatchId}`,
                type: 'get',
                success: function (res) {
                    console.log("Qty ", res);
                    realQty = res;
                    if (qty <= 0) {
                        setError('Qty must be greater than 0');
                    }
                    else if (qty > realQty) {
                        setError('Qty not enough to transfer. OnHand qty is '+realQty);
                    }
                    else {
                        setError('');
                        var selectedItem = items.find(x => x.id == selectedItemId);
                        var selectedUnit = packingUnits.find(x => x.unitId == selectedUnitId);
                        var selectedBatch = batches.find(x => x.id == selectedBatchId);

                        var transferItem = {
                            itemId: selectedItemId,
                            unitId: selectedUnitId,
                            batchId: selectedBatchId,
                            qty: Number(qty),
                            qtyInSmallestUnit: Number(calculateQtyInSmallestUnit(packingUnits, selectedUnitId, qty)),
                            itemName: selectedItem ? selectedItem.name : '',
                            unitName: selectedUnit ? selectedUnit.unitName : '',
                            batchName: selectedBatch ? selectedBatch.name : '',
                        };
                        transferItems.push(transferItem);
                        console.log("TransferItems=" + transferItem.qtyInSmallestUnit);
                        setTransferItems(JSON.parse(JSON.stringify(transferItems)));
                        console.log("SetTransferItems=" + setTransferItems.length);
                    }
                },
                error: function (jqXhr, textStatus, errorMessage) {

                }
            });
           
        } else {
            setError('* fields are required');
        }
    }

    useEffect(() => {
        //console.log("transferItems change");
        //console.log('Purchase Items', transferItems);
        debugger;
        props.reset();
        setQty('');
    }, [transferItems])

    const onDeleteItem = (index) => {
        arrayRemoveByIndex(transferItems, index);
        setTransferItems(JSON.parse(JSON.stringify(transferItems)));
    }

    return (
        <div className="col-md-12 form-group p-50">
            <div className="col-md-12">
                <h2 className="form-title pull-left">Transfer Items</h2>
                <div className="actions panel_actions pull-right">
                </div>
            </div>
            <div className="col-md-12">
                <div className="row">
                    <div className="col-md-3 form-group">
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
                    <div className="col-md-3 form-group">
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
                    <div className="col-md-3 form-group">
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
                    <div className="col-md-2 form-group">
                        <label className="form-label" for="Qty">Qty</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <input type="number" id="Qty" className="form-control" onChange={(e) => setQty(e.target.value)} value={qty} />
                        </div>
                    </div>
                    <div className="col-md-1 form-group">
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
                            sort(transferItems, "sortOrder").map((transferItem, index) => {
                                return (
                                    <tr key={index}>
                                        <td>
                                            <input name={`${transferType}[${index}].SortOrder`} value={index + 1} hidden />
                                            {index + 1}
                                        </td>
                                        <td>
                                            <input name={`${transferType}[${index}].ItemId`} value={transferItem.itemId} hidden />
                                            <input name={`${transferType}[${index}].ItemName`} value={transferItem.itemName} hidden />
                                            {transferItem.itemName || (transferItem.item ? transferItem.item.name : '')}
                                        </td>
                                        <td>
                                            <input name={`${transferType}[${index}].UnitId`} value={transferItem.unitId} hidden />
                                            <input name={`${transferType}[${index}].UnitName`} value={transferItem.unitName} hidden />
                                            {transferItem.unitName || (transferItem.unit ? transferItem.unit.name : '')}
                                        </td>
                                        <td>
                                            <input name={`${transferType}[${index}].BatchId`} value={transferItem.batchId} hidden />
                                            <input name={`${transferType}[${index}].BatchName`} value={transferItem.batchName} hidden />
                                            {transferItem.batchName || (transferItem.batch ? transferItem.batch.name : '')}
                                        </td>
                                        <td>
                                            <input name={`${transferType}[${index}].Qty`} value={transferItem.qty} hidden />
                                            <input name={`${transferType}[${index}].QtyInSmallestUnit`} value={transferItem.qtyInSmallestUnit} hidden />
                                            {transferItem.qty}
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
            outletId: _outletId && _outletId != 0 ? _outletId : undefined,
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
            <TransferItemForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)