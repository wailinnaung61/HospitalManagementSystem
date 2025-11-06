const { Component, useState, useEffect, useRef, Fragment } = React;

const Medication = (props) => {
    const [orderItems, setOrderItems] = useState([]);
    const [outlets, setOutlets] = useState([]);
    const [items, setItems] = useState([]);
    const [packingUnits, setPackingUnits] = useState([]);
    const [item, setItem] = useState({
        id: '',
        date: _selectedDate,
        iPDRecordId: _iPDRecordId,
        itemId: '',
        unitId: '',
        unitPrice: '',
        qty: '',
        qtyInSmallestUnit: '',
        isFOC: false,
        outletId: _outletId,
        sortOrder: ''
    });
    const [editId, setEditId] = useState(-1);
    const [editObjInitialized, setEditObjInitialized] = useState(true);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');

    const GetAllIPDOrderItems = () => {
        $.ajax({
            url: `/ipdrecords/GetIPDOrderItems?id=${_iPDRecordId}&date=${_selectedDate}`,
            type: 'get',
            success: function (res) {
                setOrderItems(JSON.parse(JSON.stringify(res)));
            }
        });
    }

    const GetAllOutletItems = (outletId) => {
        $.ajax({
            url: `/outlets/GetOutletItems?outletId=${outletId}`,
            type: 'get',
            success: function (res) {
                setItems(JSON.parse(JSON.stringify(res || [])));
            }
        });
    }

    useEffect(() => {
        $('#ItemModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })
        GetAllIPDOrderItems();
        $.ajax({
            url: `/outlets/GetAllOutlets`,
            type: 'get',
            success: function (res) {
                setOutlets(JSON.parse(JSON.stringify(res || [])));
            }
        });
    }, [])

    useEffect(() => {
        GetAllOutletItems(item.outletId);
    }, [item.outletId])

    useDidUpdateEffect(() => {
        if (item.itemId) {
            onChangeItem(item.itemId);
        }
    }, [items])

    const onChangeItem = (itemId) => {
        var selectedItem = items.find(x => x.id == itemId);
        if (selectedItem) {
            setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
            if (editObjInitialized) {
                setStateBulk({
                    qty: 1,
                    unitId: '',
                })
            }
        } else {
            setPackingUnits(JSON.parse(JSON.stringify([])));
            setStateBulk({
                qty: 0,
                unitId: '',
                unitPrice: ''
            })
        }
    }

    useEffect(() => {
        onChangeItem(item.itemId);
    }, [item.itemId])

    useDidUpdateEffect(() => {
        if (item.unitId && editObjInitialized) {
            var selectedUnit = packingUnits.find(x => x.unitId == item.unitId);
            setState('unitPrice', selectedUnit ? selectedUnit.saleAmount : 0);
        }
    }, [item.unitId]);

    useEffect(() => {
        if (item.unitId && editObjInitialized) {
            var _qtyInSmallestUnit = calculateQtyInSmallestUnit(packingUnits, item.unitId, item.qty);
            setState('qtyInSmallestUnit', _qtyInSmallestUnit);
        }
    }, [item.qty, item.unitId]);

    useDidUpdateEffect(() => {
        if (editObjInitialized) {
            var smallestUnit = (packingUnits && packingUnits.length > 0) ? packingUnits[packingUnits.length - 1] : null;
            setStateBulk({
                unitId: smallestUnit ? smallestUnit.unitId : '',
                unitPrice: smallestUnit ? item.isFOC ? 0 : smallestUnit.unitPrice : ''
            });
        } else {
            setEditObjInitialized(true);
        }
    }, [packingUnits]);

    const clearForm = () => {
        setItem(JSON.parse(JSON.stringify({
            id: '',
            date: _selectedDate,
            iPDRecordId: _iPDRecordId,
            itemId: '',
            unitId: '',
            unitPrice: '',
            qty: '',
            qtyInSmallestUnit: '',
            isFOC: false,
            outletId: _outletId,
            sortOrder: ''
        })));
        setEditId(-1);
    }

    const setState = (key, value) => {
        if (key == "itemId" && value) {
            var selectedItem = items.find(x => x.id == value);
            if (selectedItem) {
                item["unitPrice"] = '';
                if (!item["qty"]) item["qty"] = 1;
            }
        } else if (key == "isFOC") {
            if (value) {
                item["unitPrice"] = 0;
            }
        }
        item[key] = value;
        setItem(JSON.parse(JSON.stringify(item)));
    }

    const setStateBulk = (obj) => {
        var _item = {
            ...item,
            ...obj
        };
        setItem(JSON.parse(JSON.stringify(_item)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (item && item.itemId && item.qty && (item.isFOC || item.unitPrice > 0)) {
            if (editId > -1) {
                onEdit();
            } else {
                onAdd();
            }
        } else {
            setError('* fields are required.');
        }
    }

    const onAdd = () => {
        $.ajax({
            url: `/ipdrecords/AddOrderItem`,
            type: 'post',
            data: item,
            success: function (res) {
                // orderItems.push(item);
                // setOrderItems(JSON.parse(JSON.stringify(res)));
                GetAllIPDOrderItems();
                GetAllOutletItems(_outletId);
                setStatus('success');
                clearForm();
            }
        });
    }

    const onClickEdit = (id) => {
        var _item = JSON.parse(JSON.stringify(orderItems.find(x => x.id == id)));
        setItem(_item);
        // GetAllOutletItems(_item.outletId);
        setEditId(id);
        setEditObjInitialized(false);
        $("#ItemModal").modal('show');
    }

    const onEdit = () => {
        $.ajax({
            url: `/ipdrecords/UpdateOrderItem`,
            type: 'post',
            data: ((({ id, date, iPDRecordId, itemId, unitId, unitPrice, qty, qtyInSmallestUnit, isFOC, outletId, sortOrder }) => ({ id, date, iPDRecordId, itemId, unitId, unitPrice, qty, qtyInSmallestUnit, isFOC, outletId, sortOrder }))(item)),
            success: function (res) {
                GetAllIPDOrderItems();
                $("#ItemModal").modal('hide');
            }
        });
    }

    const onClickDelete = (id) => {
        DeleteFnc(() => onDelete(id));
    }

    const onDelete = (id) => {
        $.ajax({
            url: `/ipdrecords/DeleteOrderItem/${id}`,
            type: 'get',
            success: function (res) {
                DeleteAlert('center', 'success', 'Item');
                GetAllIPDOrderItems();
            }
        });
    }
    return (
        <div className="row">
            <div className="col-xs-12">
                <button type="button" className="btn btn-click btn-primary gradient-blue pull-right form-group" data-toggle="modal" data-target="#ItemModal">Add Item</button>
                <table id="example" className="table vm table-small-font no-mb table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Outlet</th>
                            <th>Item</th>
                            <th>Unit Price</th>
                            <th>Qty</th>
                            <th>Amount</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            orderItems.map((item, index) =>
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{item.outletName}</td>
                                    <td>{item.itemName}</td>
                                    <td>{item.isFOC ? "FOC" : item.unitPrice}</td>
                                    <td>{item.qty}</td>
                                    <td>{item.unitPrice * item.qty}</td>
                                    <td>
                                        <a onClick={() => onClickEdit(item.id)} className="btn btn-xs btn-secondary" style={{ marginRight: 3 }}>Edit</a>
                                        <a onClick={() => onClickDelete(item.id)} className="btn btn-xs btn-secondary">Delete</a>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>
            <div className="modal fade" id="ItemModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-drug pull-left">{editId > -1 ? 'Edit' : 'Add'} Item</h5>
                            <button type="button" className="close pull-right" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div className="modal-body">
                            <form>
                                <div className="row">
                                    {
                                        status === 'success' &&
                                        <div className="col-xs-12">
                                            <div className="alert alert-success alert-dismissible fade in">
                                                <button type="button" className="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button>
                                                <strong>Success:</strong> Item added successfully.</div>
                                        </div>
                                    }
                                    {
                                        error &&
                                        <div className="col-xs-12">
                                            <div className="alert alert-warning alert-dismissible fade in">
                                                <button type="button" className="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button>
                                                <strong>Warning:</strong> {error}.</div>
                                        </div>
                                    }
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Date</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input type="date" className="form-control" onChange={(e) => setState('date', e.target.value)} value={moment(item.date).format('yyyy-MM-DD')} />
                                        </div>
                                    </div>
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Outlet</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <Select
                                                isClearable={true}
                                                value={(!!item.outletId && item.outletId > 0) ? outlets.map(x => ({ label: x.name, value: x.id })).find(x => x.value == item.outletId) : null}
                                                onChange={(e) => {
                                                    // GetAllOutletItems(e.value);
                                                    setState('outletId', e ? e.value : null);
                                                    setStateBulk({
                                                        itemId: '',
                                                        unitId: '',
                                                        qty: '',
                                                        qtyInSmallestUnit: 0,
                                                        isFOC: false
                                                    });
                                                }}
                                                options={outlets.map(x => ({ label: x.name, value: x.id }))}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Item</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <Select
                                                // isClearable={true}
                                                value={(!!item.itemId && item.itemId > 0) ? items.map(x => ({ label: x.name, value: x.id })).find(x => x.value == item.itemId) : null}
                                                onChange={(e) => { setState('itemId', e ? e.value : null) }}
                                                options={items.map(x => ({ label: x.name, value: x.id }))}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Unit</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <Select
                                                // isClearable={true}
                                                value={(!!item.unitId && item.unitId > 0) ? packingUnits.map(x => ({ value: x.unitId, label: x.unitName })).find(x => x.value == item.unitId) : null}
                                                onChange={(e) => { setState('unitId', e ? e.value : null) }}
                                                options={packingUnits.map(x => ({ value: x.unitId, label: x.unitName }))}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <div className="">
                                            <input id="item_isFOC" tabindex="5" type="checkbox" className="skin-square-grey" onChange={e => setState('isFOC', e.target.checked)} checked={item.isFOC} value={item.isFOC} />
                                            <label className="icheck-label form-label" for="item_isFOC" style={{ paddingLeft: 5 }}>FOC</label>
                                        </div>
                                    </div>
                                    {
                                        !item.isFOC &&
                                        <div className="col-xs-6 form-group">
                                            <label className="form-label" for="Name">Unit Price</label>
                                            <span className="desc">*</span>
                                            <div className="controls">
                                                <input type="number" className="form-control" onChange={(e) => setState('unitPrice', e.target.value)} value={item.unitPrice} />
                                            </div>
                                        </div>
                                    }
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Qty</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input type="number" className="form-control" onChange={(e) => setState('qty', e.target.value)} value={item.qty} />
                                        </div>
                                    </div>
                                    {
                                        !item.isFOC &&
                                        <div className="col-xs-12 form-group">
                                            <label className="form-label" for="Name">Amount</label>
                                            <span className="desc"></span>
                                            <div className="controls">
                                                <input disabled className="form-control" value={item.unitPrice * item.qty} />
                                            </div>
                                        </div>
                                    }
                                </div>
                            </form>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" data-dismiss="modal">Back</button>
                            <button type="button" className="btn btn-primary" onClick={onSave}>{editId > -1 ? 'Save' : 'Add'}</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

class App extends Component {
    constructor(props) {
        super(props);

        this.state = {

        }

        this.clear = this.clear.bind(this);
    }

    componentWillUnmount() {
        this.clear();
    }

    clear = () => this.setState({});

    render() {
        return (
            <Medication {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('medication_root')
ReactDOM.render(<App ref={component => window.itemComponent = component} />, rootElement)