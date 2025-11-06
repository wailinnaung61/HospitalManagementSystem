const { Component, useState, useEffect, useRef, Fragment } = React;

const Prescription = (props) => {
    const [patientPrescriptions, setPatientPrescriptions] = useState(_prescriptions);
    const [drugs, setDrugs] = useState([]);
    const [viewName, setViewName] = useState(_viewName);
    const [prescription, setPrescription] = useState({
        id: '',
        itemId: '',
        drug: '',
        directionsForUse: '',
        day: '',
        remark: ''
    });
    const [editIndex, setEditIndex] = useState(-1);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');
    useEffect(() => {
        setViewName(viewName);
        $('#PrescriptionModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })

        $.ajax({
            url: `/items/GetAll`,
            type: 'get',
            success: function (res) {
                console.log("ITems ", res);
                setDrugs(JSON.parse(JSON.stringify(res.map(x => ({ label: x.name+' ('+x.chemicalName+')', value: x.id })) || [])));
            }
        });
    }, [])

    useEffect(() => {
        console.log(prescription)
    }, [prescription])

    const clearForm = () => {
        setPrescription(JSON.parse(JSON.stringify({
            id: '',
            itemId: '',
            drug: '',
            directionsForUse: '',
            day: '',
            remark: ''
        })));
        setEditIndex(-1);
    }

    const setState = (key, value) => {
        console.log(key, value)
        if (key == 'itemId') {
            if(value) {
                prescription['itemId'] = value.value;
                prescription['drug'] = value.label;
            }else{
                prescription['itemId'] = null;
            }
        } else {
            prescription[key] = value;
        }
        setPrescription(JSON.parse(JSON.stringify(prescription)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (prescription && prescription.drug) {
            if (editIndex > -1) {
                onEdit();
            } else {
                onAdd();
            }
        } else {
            setError('* fields are required.');
        }
    }

    const onAdd = () => {
        patientPrescriptions.push(prescription);
        setPatientPrescriptions(JSON.parse(JSON.stringify(patientPrescriptions)));
        setStatus('success');
        clearForm();
    }

    const onClickEdit = (index) => {
        setPrescription(JSON.parse(JSON.stringify(patientPrescriptions[index])));
        setEditIndex(index);
        $("#PrescriptionModal").modal('show');
    }

    const onEdit = () => {
        console.log(editIndex, prescription);
        patientPrescriptions[editIndex] = prescription;
        setPatientPrescriptions(JSON.parse(JSON.stringify(patientPrescriptions)));
        $("#PrescriptionModal").modal('hide');
    }

    const onClickDelete = (index) => {
        arrayRemoveByIndex(patientPrescriptions, index);
        setPatientPrescriptions(JSON.parse(JSON.stringify(patientPrescriptions)));
    }
    return (
        <div className="row">
            <div className="col-xs-12">
                {viewName ? <div></div> : <button type="button" className="btn btn-click btn-primary gradient-blue pull-right" data-toggle="modal" data-target="#PrescriptionModal">Add Prescription</button>

                }
                <table id="example" className="display table table-hover table-condensed">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Drug</th>
                            <th>Directions For Use</th>
                            <th>Day</th>
                            <th>Remark</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            patientPrescriptions.map((prescription, index) =>
                                <tr>
                                    <td>
                                        {/* <input name={`Prescriptions[${index}].Id`} value={prescription.id} hidden /> */}
                                        <input name={`Prescriptions[${index}].ItemId`} value={prescription.itemId} hidden />
                                        {index + 1}
                                    </td>
                                    <td><input name={`Prescriptions[${index}].Drug`} value={prescription.drug} hidden />{prescription.drug}</td>
                                    <td><input name={`Prescriptions[${index}].DirectionsForUse`} value={prescription.directionsForUse} hidden />{prescription.directionsForUse}</td>
                                    <td><input name={`Prescriptions[${index}].Day`} value={prescription.day} hidden />{prescription.day}</td>
                                    <td><input name={`Prescriptions[${index}].Remark`} value={prescription.remark} hidden />{prescription.remark}</td>
                                    {viewName ? <td></td> : <td>
                                        <a onClick={() => onClickEdit(index)} className="btn btn-xs btn-secondary" style={{ marginRight: 3 }}>Edit</a>
                                        <a onClick={() => onClickDelete(index)} className="btn btn-xs btn-secondary">Delete</a>
                                    </td>
                                    }
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>
            <div className="modal fade" id="PrescriptionModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-drug pull-left">{editIndex > -1 ? 'Edit' : 'Add'} Prescription</h5>
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
                                                <strong>Success:</strong> Prescription added successfully.</div>
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
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Prescription</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <Select
                                                
                                                isClearable={true}
                                                value={(!!prescription.itemId && prescription.itemId > 0) ? drugs.find(x => x.value == prescription.itemId) : null}
                                                onChange={(e) => { console.log(e); setState('itemId', e) }}
                                                options={drugs}/*{drugs}*/
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Drug</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input className="form-control" onChange={(e) => setState('drug', e.target.value)} value={prescription.drug} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">DirectionsForUse</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <input className="form-control" onChange={(e) => setState('directionsForUse', e.target.value)} value={prescription.directionsForUse} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Day</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <input className="form-control" onChange={(e) => setState('day', e.target.value)} value={prescription.day} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Remark</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <textarea className="form-control" onChange={(e) => setState('remark', e.target.value)} value={prescription.remark}></textarea>
                                        </div>
                                    </div>
                                </div>
                            </form>
                        </div>
                        <div className="modal-footer">
                            <button type="button" className="btn btn-secondary" data-dismiss="modal">Back</button>
                            <button type="button" className="btn btn-primary" onClick={onSave}>{editIndex > -1 ? 'Save' : 'Add'}</button>
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
            <Prescription {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('prescription_root')
ReactDOM.render(<App ref={component => window.prescriptionComponent = component} />, rootElement)