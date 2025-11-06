const { Component, useState, useEffect, useRef, Fragment } = React;

const Diagnostic = (props) => {
    const [patientDiagnostics, setPatientDiagnostics] = useState(_patientDiagnostics);
    const [viewName, setViewName] = useState(_viewName);
    const [diagnostics, setDiagnostics] = useState([]);
    const [diagnostic, setDiagnostic] = useState({
        id: '',
        diagnosticId: '',
        title: '',
        detail: ''
    });
    const [editIndex, setEditIndex] = useState(-1);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');
    useEffect(() => {
        debugger;
        console.log("ViewName:" + viewName);
        setViewName(viewName);
        $('#DiagnosticModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })

        $.ajax({
            url: `/diagnostics/GetAllByDoctor?id=${_doctorId}`,
            type: 'get',
            success: function (res) {
                setDiagnostics(JSON.parse(JSON.stringify(res.map(x => ({ label: x.name, value: x.id })) || [])));
            }
        });
    }, [])

    useEffect(() => {
        console.log(diagnostic)
    }, [diagnostic])

    const clearForm = () => {
        setDiagnostic(JSON.parse(JSON.stringify({
            id: '',
            diagnosticId: '',
            title: '',
            detail: ''
        })));
        setEditIndex(-1);
    }

    const setState = (key, value) => {
        console.log(key, value)
        if (key == 'diagnosticId') {
            if(value) {
                diagnostic['diagnosticId'] = value.value;
                diagnostic['title'] = value.label;
            }else{
                diagnostic['diagnosticId'] = null;
            }
        } else {
            diagnostic[key] = value;
        }
        setDiagnostic(JSON.parse(JSON.stringify(diagnostic)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (diagnostic && diagnostic.title) {
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
        patientDiagnostics.push(diagnostic);
        setPatientDiagnostics(JSON.parse(JSON.stringify(patientDiagnostics)));
        setStatus('success');
        clearForm();
    }

    const onClickEdit = (index) => {
        setDiagnostic(JSON.parse(JSON.stringify(patientDiagnostics[index])));
        setEditIndex(index);
        $("#DiagnosticModal").modal('show');
    }

    const onEdit = () => {
        patientDiagnostics[editIndex] = diagnostic;
        setPatientDiagnostics(JSON.parse(JSON.stringify(patientDiagnostics)));
        $("#DiagnosticModal").modal('hide');
    }

    const onClickDelete = (index) => {
        arrayRemoveByIndex(patientDiagnostics, index);
        setPatientDiagnostics(JSON.parse(JSON.stringify(patientDiagnostics)));
    }
    return (
        <div className="row">
            <div className="col-xs-12">
                {viewName ? <div></div> : <button type="button" className="btn btn-click btn-primary gradient-blue pull-right" data-toggle="modal" data-target="#DiagnosticModal">Add Diagnostic</button>

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
                            patientDiagnostics.map((diagnostic, index) =>
                                <tr>
                                    <td>
                                        {/* <input name={`PatientDiagnostics[${index}].Id`} value={diagnostic.id} hidden /> */}
                                        <input name={`PatientDiagnostics[${index}].DiagnosticId`} value={diagnostic.diagnosticId} hidden />
                                        {index + 1}
                                    </td>
                                    <td><input name={`PatientDiagnostics[${index}].Title`} value={diagnostic.title} hidden />{diagnostic.title}</td>
                                    <td><input name={`PatientDiagnostics[${index}].Detail`} value={diagnostic.detail} hidden />{diagnostic.detail}</td>
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
            <div className="modal fade" id="DiagnosticModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-title pull-left">{editIndex > -1 ? 'Edit' : 'Add'} Diagnostic</h5>
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
                                                <strong>Success:</strong> Diagnostic added successfully.</div>
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
                                        <label className="form-label" for="Name">Diagnostic</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <Select
                                                isClearable={true}
                                                value={(!!diagnostic.diagnosticId && diagnostic.diagnosticId > 0) ? diagnostics.find(x => x.value == diagnostic.diagnosticId) : null}
                                                onChange={(e) => {console.log(e);setState('diagnosticId', e)}}
                                                options={diagnostics}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Title</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input className="form-control" onChange={(e) => setState('title', e.target.value)} value={diagnostic.title} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Detail</label>
                                        <span className="desc"></span>
                                        <div className="controls">
                                            <textarea className="form-control" onChange={(e) => setState('detail', e.target.value)} value={diagnostic.detail}></textarea>
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
            <Diagnostic {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('diagnostic_root')
ReactDOM.render(<App ref={component => window.diagnosticComponent = component} />, rootElement)