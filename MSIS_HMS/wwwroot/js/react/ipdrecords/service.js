const { Component, useState, useEffect, useRef, Fragment } = React;

const Service = (props) => {
    const [orderServices, setOrderServices] = useState([]);
    const [services, setServices] = useState([]);
    const [service, setService] = useState({
        id: '',
        date: _selectedDate,
        iPDRecordId: _iPDRecordId,
        serviceId: '',
        unitPrice: '',
        qty: '',
        isFOC: false,
        sortOrder: ''
    });
    const [editId, setEditId] = useState(-1);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');

    const GetAllIPDOrderServices = () => {
        $.ajax({
            url: `/ipdrecords/GetIPDOrderServices?id=${_iPDRecordId}&date=${_selectedDate}`,
            type: 'get',
            success: function (res) {
                setOrderServices(JSON.parse(JSON.stringify(res)));
            }
        });
    }

    useEffect(() => {
        $('#ServiceModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })
        GetAllIPDOrderServices();
        $.ajax({
            url: `/services/GetAll`,
            type: 'get',
            success: function (res) {
                setServices(JSON.parse(JSON.stringify(res || [])));
            }
        });
    }, [])

    useEffect(() => {
        console.log(service)
    }, [service])

    const clearForm = () => {
        setService(JSON.parse(JSON.stringify({
            id: '',
            date: _selectedDate,
            iPDRecordId: _iPDRecordId,
            serviceId: '',
            unitPrice: '',
            qty: '',
            isFOC: false,
            sortOrder: ''
        })));
        setEditId(-1);
    }

    const setState = (key, value) => {
        if (key == "serviceId" && value) {
            var selectedService = services.find(x => x.id == value);
            console.log(services, value, selectedService)
            if (selectedService) {
                service["unitPrice"] = service["isFOC"] ? 0 : selectedService.serviceFee;
                if (!service["qty"]) service["qty"] = 1;
            }
        } else if (key == "isFOC") {
            if (value) {
                service["unitPrice"] = 0;
            }
        }
        service[key] = value;
        setService(JSON.parse(JSON.stringify(service)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (service && service.serviceId && service.qty && (service.isFOC || service.unitPrice > 0)) {
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
            url: `/ipdrecords/AddOrderService`,
            type: 'post',
            data: service,
            success: function (res) {
                // orderServices.push(service);
                // setOrderServices(JSON.parse(JSON.stringify(res)));
                GetAllIPDOrderServices();
                setStatus('success');
                clearForm();
            }
        });
    }

    const onClickEdit = (id) => {
        setService(JSON.parse(JSON.stringify(orderServices.find(x => x.id == id))));
        setEditId(id);
        $("#ServiceModal").modal('show');
    }

    const onEdit = () => {
        $.ajax({
            url: `/ipdrecords/UpdateOrderService`,
            type: 'post',
            data: service,
            success: function (res) {
                GetAllIPDOrderServices();
                $("#ServiceModal").modal('hide');
            }
        });
    }

    const onClickDelete = (id) => {
        DeleteFnc(() => onDelete(id));
    }

    const onDelete = (id) => {
        $.ajax({
            url: `/ipdrecords/DeleteOrderService/${id}`,
            type: 'get',
            success: function (res) {
                DeleteAlert('center', 'success', 'Service');
                GetAllIPDOrderServices();
            }
        });
    }
    return (
        <div className="row">
            <div className="col-xs-12">
                <button type="button" className="btn btn-click btn-primary gradient-blue pull-right form-group" data-toggle="modal" data-target="#ServiceModal">Add Service</button>
                <table id="example" className="table vm table-small-font no-mb table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Service</th>
                            <th>Unit Price</th>
                            <th>Qty</th>
                            <th>Amount</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            orderServices.map((service, index) =>
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{service.serviceName}</td>
                                    <td>{service.isFOC ? "FOC" : service.unitPrice}</td>
                                    <td>{service.qty}</td>
                                    <td>{service.unitPrice * service.qty}</td>
                                    <td>
                                        <a onClick={() => onClickEdit(service.id)} className="btn btn-xs btn-secondary" style={{ marginRight: 3 }}>Edit</a>
                                        <a onClick={() => onClickDelete(service.id)} className="btn btn-xs btn-secondary">Delete</a>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>
            <div className="modal fade" id="ServiceModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-drug pull-left">{editId > -1 ? 'Edit' : 'Add'} Service</h5>
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
                                                <strong>Success:</strong> Service added successfully.</div>
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
                                            <input type="date" className="form-control" onChange={(e) => setState('date', e.target.value)} value={moment(service.date).format('yyyy-MM-DD')} />
                                        </div>
                                    </div>
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Service</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <Select
                                                isClearable={true}
                                                value={(!!service.serviceId && service.serviceId > 0) ? services.map(x => ({ label: x.name, value: x.id })).find(x => x.value == service.serviceId) : null}
                                                onChange={(e) => { setState('serviceId', e ? e.value : null) }}
                                                options={services.map(x => ({ label: x.name, value: x.id }))}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <div className="">
                                            <input id="service_isFOC" tabindex="5" type="checkbox" className="skin-square-grey" onChange={e => setState('isFOC', e.target.checked)} checked={service.isFOC} value={service.isFOC} />
                                            <label className="icheck-label form-label" for="service_isFOC" style={{ paddingLeft: 5 }}>FOC</label>
                                        </div>
                                    </div>
                                    {
                                        !service.isFOC &&
                                        <div className="col-xs-6 form-group">
                                            <label className="form-label" for="Name">Unit Price</label>
                                            <span className="desc">*</span>
                                            <div className="controls">
                                                <input type="number" className="form-control" onChange={(e) => setState('unitPrice', e.target.value)} value={service.unitPrice} />
                                            </div>
                                        </div>
                                    }
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Qty</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input type="number" className="form-control" onChange={(e) => setState('qty', e.target.value)} value={service.qty} />
                                        </div>
                                    </div>
                                    {
                                        !service.isFOC &&
                                        <div className="col-xs-12 form-group">
                                            <label className="form-label" for="Name">Amount</label>
                                            <span className="desc"></span>
                                            <div className="controls">
                                                <input disabled className="form-control" value={service.unitPrice * service.qty} />
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
            <Service {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('service_root')
ReactDOM.render(<App ref={component => window.serviceComponent = component} />, rootElement)