
const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const OTForm = (props) => {
    const { selectedItemId, selectedUnitId, selectedBatchId } = props;
    const [items, setItems] = useState([]);
    const [services, setServices] = useState([]);
    const [referrers, setReferrers] = useState([]);
    const [doctors, setDoctors] = useState([]);
    const [aneasthetist, setAneasthetist] = useState([]);
    const [staffs, setStaffs] = useState([]);
    const [operationItems, setoperationItems] = useState(_operationItems);
    const [operationServices, setoperationServices] = useState(_operationServices);
    const [operationDoctors, setotDoctors] = useState(_otDoctors);
    const [operationStaffs, setotStaffs] = useState(_otStaffs);
    const [operationAnaesthetist, setotAnaesthetists] = useState(_otAneasthetist);
    const [total, setTotal] = useState(0);
    const [tax, setTax] = useState(_tax);
    const [discount, setDiscount] = useState(_discount);
    const [error, setError] = useState(_error);
    const [doctorTypeEnums, setdoctorTypes] = useState([]);
    const [staffTypeEnums, setstaffTypes] = useState([]);

    useEffect(() => {
        debugger;
        $.ajax({
            url: `/outlets/GetOutletItemsByUserOutlet`,
            type: 'get',
            success: function (res) {
                setItems(JSON.parse(JSON.stringify(res || [])));
                onAddItem();
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

        //$.ajax({
        //    url: `/Doctors/GetAll`,
        //    type: 'get',
        //    success: function (res) {
        //        setDoctors(JSON.parse(JSON.stringify(res || [])));
        //        onAddDoctor();
        //        setAneasthetist(JSON.parse(JSON.stringify(res || [])));
        //        onAddAnaesthetist();
        //    },
        //    error: function (jqXhr, textStatus, errorMessage) {

        //    }
        //});
        $.ajax({
            url: `/Staffs/GetAllByPosition?PositionId=${2}`,
            type: 'get',
            success: function (res) {
                setDoctors(JSON.parse(JSON.stringify(res || [])));
                onAddDoctor();
                setAneasthetist(JSON.parse(JSON.stringify(res || [])));
                onAddAnaesthetist();

            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
        $.ajax({
            url: `/Staffs/GetAllByPosition?PositionId=${1}`,
            type: 'get',
            success: function (res) {
                setStaffs(JSON.parse(JSON.stringify(res || [])));
                onAddStaff();
                
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
       

        $.ajax({
            url: `/Doctors/GetDoctorTypeEnum`,
            type: 'get',
            success: function (res) {
                setdoctorTypes(JSON.parse(JSON.stringify(res || [])));
                /* onAddStaff();*/
                console.log("Doctor Type=", res);

            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });

        $.ajax({
            url: `/Staffs/GetStaffTypeEnum`,
            type: 'get',
            success: function (res) {
                setstaffTypes(JSON.parse(JSON.stringify(res || [])));
                /* onAddStaff();*/
                //console.log("Doctor Type=", res);

            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
        
    }, []);

    useEffect(() => {
        console.log(operationItems, operationServices)
        setTotal(calculateTotal(operationItems.map(x => ({ amount: x.qty * x.unitPrice })), "amount") + calculateTotal(operationServices.map(x => ({ amount: x.qty * x.unitPrice })), "amount") + calculateTotal(operationDoctors.map(x => ({ amount: x.fee })), "amount") + calculateTotal(operationStaffs.map(x => ({ amount: x.fee })), "amount"));
    }, [operationItems, operationServices, operationDoctors, operationStaffs])

    const onAddItem = () => {
        var orderItem = {
            itemId: '',
            unitId: '',
            qty: 0,
            qtyInSmallestUnit: 0,
            unitPrice: 0,
            isFOC: false
        };
        operationItems.push(orderItem);
        setoperationItems(JSON.parse(JSON.stringify(operationItems)));
    }

    const onAddDoctor = () => {
        var otDoctor = {
            doctorId: '',
            otDoctorTypeEnum:'',
            fee: 0
         
        };
        operationDoctors.push(otDoctor);
        setotDoctors(JSON.parse(JSON.stringify(operationDoctors)));
    }
    const onAddAnaesthetist = () => {
        var otAnaesthetist = {
            doctorId: '',
            otDoctorTypeEnum: '',
            fee: 0

        };
        operationAnaesthetist.push(otAnaesthetist);
        setotAnaesthetists(JSON.parse(JSON.stringify(operationAnaesthetist)));
    }
    
    const onAddStaff = () => {
        var otStaff = {
            staffId: '',
            otStaffTypeEnum:'',
            fee: 0

        };
        operationStaffs.push(otStaff);
        setotStaffs(JSON.parse(JSON.stringify(operationStaffs)));
    }
    const onUpdateItem = (index, key, value) => {
        operationItems[index][key] = value;
        setoperationItems(JSON.parse(JSON.stringify(operationItems)));
    }

    const onDeleteItem = (index) => {
        arrayRemoveByIndex(operationItems, index);
        setoperationItems(JSON.parse(JSON.stringify(operationItems)));
    }
    const onDeleteDoctor = (index) => {
        arrayRemoveByIndex(operationDoctors, index);
        setotDoctors(JSON.parse(JSON.stringify(operationDoctors)));
    }
    const onDeleteAnaesthetist = (index) => {
        arrayRemoveByIndex(operationAnaesthetist, index);
        setotAnaesthetists(JSON.parse(JSON.stringify(operationAnaesthetist)));
    }
    const onDeleteStaff = (index) => {
        arrayRemoveByIndex(operationStaffs, index);
        setotStaffs(JSON.parse(JSON.stringify(operationStaffs)));
    }
    const onUpdateDoctor = (index, key, value) => {
        operationDoctors[index][key] = value;
        setotDoctors(JSON.parse(JSON.stringify(operationDoctors)));
    }
    const onUpdateAnaesthetist = (index, key, value) => {
        operationAnaesthetist[index][key] = value;
        setotAnaesthetists(JSON.parse(JSON.stringify(operationAnaesthetist)));
    }
    const onUpdateStaff = (index, key, value) => {
        operationStaffs[index][key] = value;
        setotStaffs(JSON.parse(JSON.stringify(operationStaffs)));
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
        operationServices.push(orderService);
        setoperationServices(JSON.parse(JSON.stringify(operationServices)));
    }

    const onUpdateService = (index, key, value) => {
        console.log(index, key, value)
        operationServices[index][key] = value;
        setoperationServices(JSON.parse(JSON.stringify(operationServices)));
    }

    const onDeleteService = (index) => {
        arrayRemoveByIndex(operationServices, index);
        setoperationServices(JSON.parse(JSON.stringify(operationServices)));
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
                        sort(operationItems, "sortOrder").map((orderItem, index) => {
                            return (
                                <OrderItemRow key={index}
                                    orderItem={orderItem}
                                    index={index}
                                    items={items}
                                    onUpdateItem={onUpdateItem}
                                    onAddItem={onAddItem}
                                    onDeleteItem={onDeleteItem}
                                    isLastItem={index == (operationItems.length - 1)}
                                />
                            )
                        })
                       
                    }
                </tbody>
            </table>
            <table className="display table table-hover table-condensed">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th width="30%">Service</th>
                        <th width="20%"></th>
                        <th width="15%" className="money">Unit Price</th>
                        <th width="10%">Qty</th>
                        <th width="15%" className="money">Amount</th>
                        <th width="10%" style={{ textAlign: 'center' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        sort(operationServices, "sortOrder").map((orderService, index) => {
                            return (
                                <OrderServiceRow key={index}
                                    orderService={orderService}
                                    //referrers={referrers}
                                    index={index}
                                    services={services}
                                    onUpdateService={onUpdateService}
                                    onAddService={onAddService}
                                    onDeleteService={onDeleteService}
                                    isLastService={index == (operationServices.length - 1)}
                                />
                            )
                        })
                    }
                </tbody>
                
            </table>
            <table className="display table table-hover table-condensed">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th width="30%">Assistant Doctor For Chief Surgeon</th>
                        <th width="20%"></th>
                        <th width="15%" ></th>
                        <th width="10%" >Fee</th>
                        <th width="15%" className="money">Amount</th>
                        <th width="10%" style={{ textAlign: 'center' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        sort(operationDoctors, "sortOrder").map((otDoctor, index) => {
                            return (
                                <OTDoctorRow key={index}
                                    otDoctor={otDoctor}
                                    //referrers={referrers}
                                    index={index}
                                    doctors={doctors}
                                    onUpdateDoctor={onUpdateDoctor}
                                    onAddDoctor={onAddDoctor}
                                    onDeleteDoctor={onDeleteDoctor}
                                    isLastDoctor={index == (operationDoctors.length - 1)}
                                    otDoctorTypeEnum={doctorTypeEnums}
                                />
                            )
                        })
                    }
                </tbody>
              
            </table>
            <table className="display table table-hover table-condensed">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th width="30%">Assistant Doctor For Aneasthetist</th>
                        <th width="20%"></th>
                        <th width="15%" ></th>
                        <th width="10%" >Fee</th>
                        <th width="15%" className="money">Amount</th>
                        <th width="10%" style={{ textAlign: 'center' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        sort(operationAnaesthetist, "sortOrder").map((otAnaesthetist, index) => {
                            return (
                                <OTAnaesthetistRow key={index}
                                    otAnaesthetist={otAnaesthetist}
                                    //referrers={referrers}
                                    index={index}
                                    doctors={doctors}
                                    onUpdateAnaesthetist={onUpdateAnaesthetist}
                                    onAddAnaesthetist={onAddAnaesthetist}
                                    onDeleteAnaesthetist={onDeleteAnaesthetist}
                                    isLastDoctor={index == (operationAnaesthetist.length - 1)}
                                    otDoctorTypeEnum={doctorTypeEnums}
                                />
                            )
                        })
                    }
                </tbody>

            </table>
            <table className="display table table-hover table-condensed">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th width="30%">Staff</th>
                        <th width="20%"></th>
                        <th width="15%" ></th>
                        <th width="10%" >Fee</th>
                        <th width="15%" className="money">Amount</th>
                        <th width="10%" style={{ textAlign: 'center' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        sort(operationStaffs, "sortOrder").map((otStaff, index) => {
                            return (
                                <OTStaffRow key={index}
                                    otStaff={otStaff}
                                    //referrers={referrers}
                                    index={index}
                                    staffs={staffs}
                                    onUpdateStaff={onUpdateStaff}
                                    onAddStaff={onAddStaff}
                                    onDeleteStaff={onDeleteStaff}
                                    isLastStaff={index == (operationStaffs.length - 1)}
                                    otStaffTypeEnum={staffTypeEnums}
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
                <input type="number" name={`${name}Qty`} className="form-control" onChange={(e) => onUpdateItem(index, 'qty', e.target.value)} value={orderItem.qty} />
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
    debugger;
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
    console.log("Services=", (orderService.qty || 0) * (orderService.unitPrice || 0));
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
                {/*<input name={`${name}ReferrerId`} value={orderService.referrerId} hidden />*/}
                {/*<Select*/}
                {/*    isClearable={true}*/}
                {/*    value={referrers.map(x => ({ value: x.id, label: `${x.name}` })).find(x => x.value == orderService.referrerId)}*/}
                {/*    onChange={(e) => onUpdateService(index, 'referrerId', e ? e.value : '')}*/}
                {/*    options={referrers.map(x => ({ value: x.id, label: `${x.name}` }))}*/}
                {/*/>*/}
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

const OTDoctorRow = (props) => {
    debugger;
    const { doctors, index, otDoctor, onUpdateDoctor, onAddDoctor, onDeleteDoctor, isLastDoctor,otDoctorTypeEnum } = props;
    const [packingUnits, setPackingUnits] = useState([]);

    console.log("otDoctorTypeEnum", otDoctorTypeEnum);
    useDidUpdateEffect(() => {
        var selectedItem = doctors.find(x => x.id == otDoctor.doctorId);
        if (selectedItem) {
           // setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
            //if (orderItem.qty == 0) {
            //    onUpdateItem(index, 'qty', 1);
            //}
            if (isLastDoctor) {
                onAddDoctor();
            }
        } else {
            //setPackingUnits(JSON.parse(JSON.stringify([])));
            onUpdateDoctor(index, 'fee', 0);
        }
    }, [otDoctor.doctorId]);

    var name = otDoctor && otDoctor.doctorId && otDoctor.doctorId > 0 ? `${_otDoctorType}[${index}].` : '';

  
    return (
        <tr>
            <td>
                <input name={`${name}SortOrder`} value={index + 1} hidden />
                {index + 1}
            </td>
            <td>
                <input name={`${name}DoctorId`} value={otDoctor.doctorId} hidden />
                <Select
                    value={isLastDoctor ? null : doctors.map(x => ({ value: x.id, label: `${x.name} (${x.code})` })).find(x => x.value == otDoctor.doctorId)}
                    onChange={(e) => onUpdateDoctor(index, 'doctorId', e.value)}
                    options={doctors.map(x => ({ value: x.id, label: `${x.name} (${x.code})` }))}
                />
            </td>
            <td>
                <input name={`${name}OTDoctorTypeEnum`} value={otDoctor.otDoctorTypeEnum} hidden />
                <Select
                    isClearable={true}
                    value={otDoctorTypeEnum.map(x => ({ value: x.value, label: `${x.name}` })).find(x => x.value == otDoctor.otDoctorTypeEnum)}
                    onChange={(e) => onUpdateDoctor(index, 'otDoctorTypeEnum', e ? e.value : '')}
                    options={otDoctorTypeEnum.map(x => ({ value: x.value, label: `${x.name}` }))}
                />
                
            </td>
            <td >
                
            </td>
            <td>
                <input type="number" name={`${name}Fee`} className="form-control" onChange={(e) => onUpdateDoctor(index, 'fee', e.target.value)} value={otDoctor.fee} />
            </td>
            <td className="money pt-td">{Number(otDoctor.fee).formatMoney()}</td>
            <td className="pt-td">
                {
                    !isLastDoctor && <button type="button" onClick={() => onDeleteDoctor(index)} className="btn btn-xs btn-secondary">Delete</button>
                }
            </td>
        </tr>
    )
}

const OTAnaesthetistRow = (props) => {
    debugger;
    const { doctors, index, otAnaesthetist, onUpdateAnaesthetist, onAddAnaesthetist, onDeleteAnaesthetist, isLastDoctor, otDoctorTypeEnum } = props;
    const [packingUnits, setPackingUnits] = useState([]);

    console.log("otDoctorTypeEnum", otDoctorTypeEnum);
    useDidUpdateEffect(() => {
        var selectedItem = doctors.find(x => x.id == otAnaesthetist.doctorId);
        if (selectedItem) {
            // setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
            //if (orderItem.qty == 0) {
            //    onUpdateItem(index, 'qty', 1);
            //}
            if (isLastDoctor) {
                onAddAnaesthetist();
            }
        } else {
            //setPackingUnits(JSON.parse(JSON.stringify([])));
            onUpdateAnaesthetist(index, 'fee', 0);
        }
    }, [otAnaesthetist.doctorId]);

    var name = otAnaesthetist && otAnaesthetist.doctorId && otAnaesthetist.doctorId > 0 ? `${_otAnaesthetistType}[${index}].` : '';


    return (
        <tr>
            <td>
                <input name={`${name}SortOrder`} value={index + 1} hidden />
                {index + 1}
            </td>
            <td>
                <input name={`${name}DoctorId`} value={otAnaesthetist.doctorId} hidden />
                <Select
                    value={isLastDoctor ? null : doctors.map(x => ({ value: x.id, label: `${x.name} (${x.code})` })).find(x => x.value == otAnaesthetist.doctorId)}
                    onChange={(e) => onUpdateAnaesthetist(index, 'doctorId', e.value)}
                    options={doctors.map(x => ({ value: x.id, label: `${x.name} (${x.code})` }))}
                />
            </td>
            <td>
                <input name={`${name}OTDoctorTypeEnum`} value={otAnaesthetist.otDoctorTypeEnum} hidden />
                <Select
                    isClearable={true}
                    value={otDoctorTypeEnum.map(x => ({ value: x.value, label: `${x.name}` })).find(x => x.value == otAnaesthetist.otDoctorTypeEnum)}
                    onChange={(e) => onUpdateAnaesthetist(index, 'otDoctorTypeEnum', e ? e.value : '')}
                    options={otDoctorTypeEnum.map(x => ({ value: x.value, label: `${x.name}` }))}
                />

            </td>
            <td >

            </td>
            <td>
                <input type="number" name={`${name}Fee`} className="form-control" onChange={(e) => onUpdateAnaesthetist(index, 'fee', e.target.value)} value={otAnaesthetist.fee} />
            </td>
            <td className="money pt-td">{Number(otAnaesthetist.fee).formatMoney()}</td>
            <td className="pt-td">
                {
                    !isLastDoctor && <button type="button" onClick={() => onDeleteAnaesthetist(index)} className="btn btn-xs btn-secondary">Delete</button>
                }
            </td>

        </tr>
    )
}
const OTStaffRow = (props) => {
    debugger;
    const { staffs, index, otStaff, onUpdateStaff, onAddStaff, onDeleteStaff, isLastStaff, otStaffTypeEnum } = props;
    const [packingUnits, setPackingUnits] = useState([]);


    useDidUpdateEffect(() => {
        var selectedItem = staffs.find(x => x.id == otStaff.staffId);
        if (selectedItem) {
            // setPackingUnits(JSON.parse(JSON.stringify(selectedItem.packingUnits)));
            //if (orderItem.qty == 0) {
            //    onUpdateItem(index, 'qty', 1);
            //}
            if (isLastStaff) {
                onAddStaff();
            }
        } else {
            //setPackingUnits(JSON.parse(JSON.stringify([])));
            onUpdateStaff(index, 'fee', 0);
        }
    }, [otStaff.staffId]);

    var name = otStaff && otStaff.staffId && otStaff.staffId > 0 ? `${_otStaffType}[${index}].` : '';


    return (
        <tr>
            <td>
                <input name={`${name}SortOrder`} value={index + 1} hidden />
                {index + 1}
            </td>
            <td>
                <input name={`${name}StaffId`} value={otStaff.staffId} hidden />
                <Select
                    value={isLastStaff ? null : staffs.map(x => ({ value: x.id, label: `${x.name} (${x.positionName})` })).find(x => x.value == otStaff.staffId)}
                    onChange={(e) => onUpdateStaff(index, 'staffId', e.value)}
                    options={staffs.map(x => ({ value: x.id, label: `${x.name} (${x.positionName})` }))}
                />
            </td>
            <td>
                <input name={`${name}OTStaffTypeEnum`} value={otStaff.otStaffTypeEnum} hidden />
                <Select
                    isClearable={true}
                    value={otStaffTypeEnum.map(x => ({ value: x.value, label: `${x.name}` })).find(x => x.value == otStaff.otStaffTypeEnum)}
                    onChange={(e) => onUpdateStaff(index, 'otStaffTypeEnum', e ? e.value : '')}
                    options={otStaffTypeEnum.map(x => ({ value: x.value, label: `${x.name}` }))}
                />
            </td>
            <td >

            </td>
            <td>
                <input type="number" name={`${name}Fee`} className="form-control" onChange={(e) => onUpdateStaff(index, 'fee', e.target.value)} value={otStaff.fee} />
            </td>
            <td className="money pt-td">{Number(otStaff.fee).formatMoney()}</td>
            <td className="pt-td">
                {
                    !isLastStaff && <button type="button" onClick={() => onDeleteStaff(index)} className="btn btn-xs btn-secondary">Delete</button>
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
            <OTForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)