const { Component, useState, useEffect, useRef, Fragment } = React;

const Symptom = (props) => {
    const [patientSymptoms, setPatientSymptoms] = useState(_patientSymptoms);
    const [viewName,setViewName] = useState(_viewName);
    const [symptoms, setSymptoms] = useState([]);
    const [symptom, setSymptom] = useState({
        id: '',
        symptomId: '',
        title: '',
        fromDate: '',
        detail: ''
    });
    const [editIndex, setEditIndex] = useState(-1);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');
    useEffect(() => {
        console.log("ViewName:" + viewName);
        setViewName (viewName);
        $('#SymptomModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })

        $.ajax({
            url: `/symptoms/GetAllByDoctor?id=${_doctorId}`,
            type: 'get',
            success: function (res) {
                setSymptoms(JSON.parse(JSON.stringify(res.map(x => ({ label: x.name, value: x.id })) || [])));
            }
        });
    }, [])

    useEffect(() => {
        console.log(symptom)
    }, [symptom])

    const clearForm = () => {
        setSymptom(JSON.parse(JSON.stringify({
            id: '',
            symptomId: '',
            title: '',
            fromDate: '',
            detail: ''
        })));
        setEditIndex(-1);
    }

    const setState = (key, value) => {
        console.log(key, value)
        if (key == 'symptomId') {
            if(value) {
                symptom['symptomId'] = value.value;
                symptom['title'] = value.label;
            }else{
                symptom['symptomId'] = null;
            }
        } else {
            symptom[key] = value;
        }
        setSymptom(JSON.parse(JSON.stringify(symptom)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (symptom && symptom.title) {
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
        patientSymptoms.push(symptom);
        setPatientSymptoms(JSON.parse(JSON.stringify(patientSymptoms)));
        setStatus('success');
        clearForm();
    }

    const onClickEdit = (index) => {
        setSymptom(JSON.parse(JSON.stringify(patientSymptoms[index])));
        setEditIndex(index);
        $("#SymptomModal").modal('show');
    }

    const onEdit = () => {
        patientSymptoms[editIndex] = symptom;
        setPatientSymptoms(JSON.parse(JSON.stringify(patientSymptoms)));
        $("#SymptomModal").modal('hide');
    }

    const onClickDelete = (index) => {
        arrayRemoveByIndex(patientSymptoms, index);
        setPatientSymptoms(JSON.parse(JSON.stringify(patientSymptoms)));
    }
    return (
          
        <div className="row">
            <div className="col-xs-12">
                {viewName ? <div></div> : <button type="button" className="btn btn-click btn-primary gradient-blue pull-right" data-toggle="modal" data-target="#SymptomModal">Add Symptom</button>
                   
                }              
                <table id="example" className="display table table-hover table-condensed">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Title</th>
                            <th>Detail</th>
                            <th>From Date</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            patientSymptoms.map((symptom, index) =>
                                <tr>
                                    <td>
                                        {/* <input name={`PatientSymptoms[${index}].Id`} value={symptom.id} hidden /> */}
                                        <input name={`PatientSymptoms[${index}].SymptomId`} value={symptom.symptomId} hidden />
                                        {index + 1}
                                    </td>
                                    <td><input name={`PatientSymptoms[${index}].Title`} value={symptom.title} hidden />{symptom.title}</td>
                                    <td><input name={`PatientSymptoms[${index}].FromDate`} value={symptom.fromDate} hidden />{symptom.fromDate}</td>
                                    <td><input name={`PatientSymptoms[${index}].Detail`} value={symptom.detail} hidden />{symptom.detail}</td>
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
            <div className="modal fade" id="SymptomModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title pull-left">{editIndex > -1 ? 'Edit' : 'Add'} Symptom</h5>
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
                                                <strong>Success:</strong> Symptom added successfully.</div>
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
                                        <label className="form-label" for="Name">Symptom</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <Select
                                                isClearable={true}
                                                value={(!!symptom.symptomId && symptom.symptomId > 0) ? symptoms.find(x => x.value == symptom.symptomId) : null}
                                                onChange={(e) => {console.log(e);setState('symptomId', e)}}
                                                options={symptoms}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Title</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input className="form-control" onChange={(e) => setState('title', e.target.value)} value={symptom.title} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">From Date</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <input className="form-control" onChange={(e) => setState('fromDate', e.target.value)} value={symptom.fromDate} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Detail</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <textarea className="form-control" onChange={(e) => setState('detail', e.target.value)} value={symptom.detail}></textarea>
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
            <Symptom {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('symptom_root')
ReactDOM.render(<App ref={component => window.symptomComponent = component} />, rootElement)