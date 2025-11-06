const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const OrderForm = (props) => {
    const { selectedItemId, selectedUnitId, selectedBatchId } = props;
    const [items, setItems] = useState([]);
    const [services, setServices] = useState([]);
    const [referrers, setReferrers] = useState([]);
    const [orderItems, setOrderItems] = useState(_orderItems);
    const [orderServices, setOrderServices] = useState(_orderServices);
    const [total, setTotal] = useState(0);
    const [tax, setTax] = useState(_tax);
    const [discount, setDiscount] = useState(_discount);
    const [error, setError] = useState(_error);

    useEffect(() => {
        debugger;
        $.ajax({
            url: `/outlets/GetOutletItemsByUserOutlet`,
            type: 'get',
            success: function (res) {
                setItems(JSON.parse(JSON.stringify(res || [])));
                onAddItem();
                console.log("Items ", items);
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });

        $.ajax({
            url: `/services/GetAll`,
            type: 'get',
            success: function (res) {
                setServices(JSON.parse(JSON.stringify(res || [])));
                onAddService();
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });

        $.ajax({
            url: `/referrers/GetAll`,
            type: 'get',
            success: function (res) {
                setReferrers(JSON.parse(JSON.stringify(res || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
    }, []);

    useEffect(() => {
        console.log(orderItems, orderServices)
        setTotal(calculateTotal(orderItems.map(x => ({ amount: x.qty * x.unitPrice })), "amount") + calculateTotal(orderServices.map(x => ({ amount: x.qty * x.unitPrice })), "amount"));
    }, [orderItems, orderServices])

    const onAddItem = () => {
        var orderItem = {
            itemId: '',
            unitId: '',
            qty: 0,
            qtyInSmallestUnit: 0,
            unitPrice: 0,
            isFOC: false
        };
        orderItems.push(orderItem);
        setOrderItems(JSON.parse(JSON.stringify(orderItems)));
    }

    const onUpdateItem = (index, key, value) => {
        orderItems[index][key] = value;
        setOrderItems(JSON.parse(JSON.stringify(orderItems)));
    }

    const onDeleteItem = (index) => {
        arrayRemoveByIndex(orderItems, index);
        setOrderItems(JSON.parse(JSON.stringify(orderItems)));
    }

    const onAddService = () => {
        var orderService = {
            serviceId: '',
            referrerId: '',
            qty: 0,
            feeType: 0,
            fee: 0,
            referralFee: 0,
            unitPrice: 0,
            isFOC: false
        };
        orderServices.push(orderService);
        setOrderServices(JSON.parse(JSON.stringify(orderServices)));
    }

    const onUpdateService = (index, key, value) => {
        console.log(index, key, value)
        orderServices[index][key] = value;
        setOrderServices(JSON.parse(JSON.stringify(orderServices)));
    }

    const onDeleteService = (index) => {
        arrayRemoveByIndex(orderServices, index);
        setOrderServices(JSON.parse(JSON.stringify(orderServices)));
    }

    return (
        <div className="col-md-12 form-group">
            {
                error && <span className="text-danger mb-5">{error}</span>
            }
            <table className="display table table-hover table-condensed">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th width="30%">Item</th>
                        <th width="20%">Unit</th>
                        <th width="15%" className="money">Unit Price</th>
                        <th width="10%">Qty</th>
                        <th width="15%" className="money">Amount</th>
                        <th width="10%" style={{ textAlign: 'center' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        sort(orderItems, "sortOrder").map((orderItem, index) => {
                            return (
                                <OrderItemRow key={index}
                                    orderItem={orderItem}
                                    index={index}
                                    items={items}
                                    onUpdateItem={onUpdateItem}
                                    onAddItem={onAddItem}
                                    onDeleteItem={onDeleteItem}
                                    isLastItem={index == (orderItems.length - 1)}
                                />
                            )
                        })
                        //<tr>
                        //    <td></td>
                        //    <td colSpan={6}>
                        //        <button type="button" onClick={onAddItem} className="btn btn-primary"><i className="fa fa-plus"></i> Add Item</button>
                        //    </td>
                        //</tr>
                    }
                </tbody>
            </table>
            <table className="display table table-hover table-condensed">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th width="30%">Service</th>
                        <th width="20%">Referrer</th>
                        <th width="15%" className="money">Unit Price</th>
                        <th width="10%">Qty</th>
                        <th width="15%" className="money">Amount</th>
                        <th width="10%" style={{ textAlign: 'center' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        sort(orderServices, "sortOrder").map((orderService, index) => {
                            return (
                                <OrderServiceRow key={index}
                                    orderService={orderService}
                                    referrers={referrers}
                                    index={index}
                                    services={services}
                                    onUpdateService={onUpdateService}
                                    onAddService={onAddService}
                                    onDeleteService={onDeleteService}
                                    isLastService={index == (orderServices.length - 1)}
                                />
                            )
                        })
                    }
                </tbody>
                <tfoot>
                    <tr><td colSpan={7}></td></tr>
                    <tr>
                        <td colSpan={5} className="money">Sub Total</td>
                        <td className="money">{total.formatMoney()}</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={5} className="money pt-td">Tax</td>
                        <td><input type="number" name={`Tax`} className="form-control money-input" onChange={(e) => setTax(Number(e.target.value))} value={tax} /></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={5} className="money pt-td">Discount</td>
                        <td><input type="number" name={`Discount`} className="form-control money-input" onChange={(e) => setDiscount(Number(e.target.value))} value={discount} /></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={5} className="money">Grand Total</td>
                        <td className="money">{(Number(total) + Number(tax) - Number(discount)).formatMoney()}</td>
                        <td></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    );
}

const OrderItemRow = (props) => {
    const { items, index, orderItem, onUpdateItem, onAddItem, onDeleteItem, isLastItem } = props;
    const [packingUnits, setPackingUnits] = useState([]);

    useEffect(() => {
        var selectedItem = items.find(x => x.id == orderItem.itemId);
        if (selectedItem) {
            setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
            if (isLastItem) {
                onAddItem();
            }
        } else {
            setPackingUnits(JSON.parse(JSON.stringify([])));
        }
    }, [items])

    useDidUpdateEffect(() => {
        var selectedItem = items.find(x => x.id == orderItem.itemId);
        if (selectedItem) {
            setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
            if (orderItem.qty == 0) {
                onUpdateItem(index, 'qty', 1);
            }
            if (isLastItem) {
                onAddItem();
            }
        } else {
            setPackingUnits(JSON.parse(JSON.stringify([])));
            onUpdateItem(index, 'qty', 0);
        }
    }, [orderItem.itemId]);

    useDidUpdateEffect(() => {
        var selectedUnit = packingUnits.find(x => x.unitId == orderItem.unitId);
        onUpdateItem(index, 'unitPrice', selectedUnit ? selectedUnit.saleAmount : 0);
    }, [orderItem.unitId]);

    useEffect(() => {
        if (orderItem.unitId) {
            var _qtyInSmallestUnit = calculateQtyInSmallestUnit(packingUnits, orderItem.unitId, orderItem.qty);
            onUpdateItem(index, 'qtyInSmallestUnit', _qtyInSmallestUnit);
        }
    }, [orderItem.qty, orderItem.unitId]);

    useDidUpdateEffect(() => {
        if (!orderItem.unitId) {
            var smallestUnit = (packingUnits && packingUnits.length > 0) ? packingUnits[packingUnits.length - 1] : null;
            onUpdateItem(index, 'unitId', smallestUnit ? smallestUnit.unitId : '');
        }
    }, [packingUnits]);

    var name = orderItem && orderItem.itemId && orderItem.itemId > 0 ? `${_itemType}[${index}].` : '';

    return (
        <tr>
            <td>
                <input name={`${name}SortOrder`} value={index + 1} hidden />
                {index + 1}
            </td>
            <td>
                <input name={`${name}ItemId`} value={orderItem.itemId} hidden />
                <Select
                    autoFocus={isLastItem}
                    value={isLastItem ? null : items.map(x => ({ value: x.id, label: `${x.name} (${x.chemicalName})` })).find(x => x.value == orderItem.itemId)}
                    onChange={(e) => onUpdateItem(index, 'itemId', e.value)}
                    options={items.map(x => ({ value: x.id, label: `${x.name} (${x.chemicalName})` }))}
                />
            </td>
            <td>
                <input name={`${name}UnitId`} value={orderItem.unitId} hidden />
                <Select
                    value={packingUnits.map(x => ({ value: x.unitId, label: x.unitName })).find(x => x.value == orderItem.unitId)}
                    onChange={(e) => onUpdateItem(index, 'unitId', e.value)}
                    options={packingUnits.map(x => ({ value: x.unitId, label: x.unitName }))}
                />
            </td>
            <td className="money pt-td">
                <input name={`${name}IsFOC`} value={orderItem.isFOC} hidden />
                <input name={`${name}UnitPrice`} value={orderItem.unitPrice} hidden />
                {orderItem.isFOC ? 'FOC' : orderItem.unitPrice.formatMoney()}
            </td>
            <td>
                <input name={`${name}QtyInSmallestUnit`} value={orderItem.qtyInSmallestUnit} hidden />
                <input type="number" name={`${name}Qty`} className="form-control" onChange={(e) => onUpdateItem(index, 'qty', e.target.value)} onKeyDown={e => {
                    if (e.key === 'Enter') {
                        console.log('do validate');
                    }
                }} value={orderItem.qty} />
            </td>
            <td className="money pt-td">{((orderItem.qty || 0) * (orderItem.unitPrice || 0)).formatMoney()}</td>
            <td className="pt-td">
                {
                    !isLastItem && <button type="button" onClick={() => onDeleteItem(index)} className="btn btn-xs btn-secondary">Delete</button>
                }
            </td>
        </tr>
    )
}

const OrderServiceRow = (props) => {
    const { services, referrers, index, orderService, onUpdateService, onAddService, onDeleteService, isLastService } = props;
    const [packingUnits, setPackingUnits] = useState([]);

    useDidUpdateEffect(() => {
        var selectedService = services.find(x => x.id == orderService.serviceId);
        if (selectedService) {
            onUpdateService(index, 'unitPrice', selectedService.serviceFee);
            if (orderService.qty == 0) {
                onUpdateService(index, 'qty', 1);
            }
            if (isLastService) {
                onAddService();
            }
        } else {
            onUpdateService(index, 'qty', 0);
        }
    }, [orderService.serviceId])

    var name = orderService && orderService.serviceId && orderService.serviceId > 0 ? `${_serviceType}[${index}].` : '';

    return (
        <tr>
            <td>
                <input name={`${name}SortOrder`} value={index + 1} hidden />
                {index + 1}
            </td>
            <td>
                <input name={`${name}ServiceId`} value={orderService.serviceId} hidden />
                <Select
                    value={isLastService ? null : services.map(x => ({ value: x.id, label: `${x.name} (${x.code})` })).find(x => x.value == orderService.serviceId)}
                    onChange={(e) => onUpdateService(index, 'serviceId', e.value)}
                    options={services.map(x => ({ value: x.id, label: `${x.name} (${x.code})` }))}
                />
            </td>
            <td>
                <input name={`${name}ReferrerId`} value={orderService.referrerId} hidden />
                <Select
                    isClearable={true}
                    value={referrers.map(x => ({ value: x.id, label: `${x.name}` })).find(x => x.value == orderService.referrerId)}
                    onChange={(e) => onUpdateService(index, 'referrerId', e ? e.value : '')}
                    options={referrers.map(x => ({ value: x.id, label: `${x.name}` }))}
                />
            </td>
            <td className="money pt-td">
                <input name={`${name}IsFOC`} value={orderService.isFOC} hidden />
                <input name={`${name}UnitPrice`} value={orderService.unitPrice} hidden />
                {orderService.isFOC ? 'FOC' : orderService.unitPrice.formatMoney()}
            </td>
            <td>
                <input type="number" name={`${name}Qty`} className="form-control" onChange={(e) => onUpdateService(index, 'qty', e.target.value)} value={orderService.qty} />
            </td>
            <td className="money pt-td">{((orderService.qty || 0) * (orderService.unitPrice || 0)).formatMoney()}</td>
            <td className="pt-td">
                {
                    !isLastService && <button type="button" onClick={() => onDeleteService(index)} className="btn btn-xs btn-secondary">Delete</button>
                }
            </td>
        </tr>
    )
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
            <OrderForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)