const { Component, useState, useEffect, useRef, Fragment } = React;

const Round = (props) => {
    const [rounds, setRounds] = useState([]);
    const [doctors, setDoctors] = useState([]);
    const [round, setRound] = useState({
        id: '',
        date: _selectedDate,
        iPDRecordId: _iPDRecordId,
        doctorId: '',
        fee: '',
        isFOC: false,
        sortOrder: ''
    });
    const [editId, setEditId] = useState(-1);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');

    const GetAllIPDRounds = () => {
        $.ajax({
            url: `/ipdrecords/GetIPDRounds?id=${_iPDRecordId}&date=${_selectedDate}`,
            type: 'get',
            success: function (res) {
                setRounds(JSON.parse(JSON.stringify(res)));
            }
        });
    }

    useEffect(() => {
        $('#RoundModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })
        GetAllIPDRounds();
        $.ajax({
            url: `/doctors/GetAll?DepartmentType=2`,
            type: 'get',
            success: function (res) {
                setDoctors(JSON.parse(JSON.stringify(res || [])));
            }
        });
    }, [])

    useEffect(() => {
        console.log(round)
    }, [round])

    const clearForm = () => {
        setRound(JSON.parse(JSON.stringify({
            id: '',
            date: _selectedDate,
            iPDRecordId: _iPDRecordId,
            doctorId: '',
            fee: '',
            isFOC: false,
            sortOrder: ''
        })));
        setEditId(-1);
    }

    const setState = (key, value) => {
        if (key == "doctorId") {
            if (value) {
                var selectedRound = doctors.find(x => x.id == value);
                console.log(doctors, value, selectedRound)
                if (selectedRound) {
                    round["fee"] = round["isFOC"] ? 0 : (selectedRound.roundFee || '');
                }
            } else {
                round["fee"] = '';
            }
        } else if (key == "isFOC") {
            if (value) {
                round["fee"] = 0;
            }
        }
        round[key] = value;
        setRound(JSON.parse(JSON.stringify(round)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (round && round.doctorId && (round.isFOC || round.fee > 0)) {
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
            url: `/ipdrecords/AddRound`,
            type: 'post',
            data: round,
            success: function (res) {
                // rounds.push(round);
                // setRounds(JSON.parse(JSON.stringify(res)));
                GetAllIPDRounds();
                setStatus('success');
                clearForm();
            }
        });
    }

    const onClickEdit = (id) => {
        setRound(JSON.parse(JSON.stringify(rounds.find(x => x.id == id))));
        setEditId(id);
        $("#RoundModal").modal('show');
    }

    const onEdit = () => {
        $.ajax({
            url: `/ipdrecords/UpdateRound`,
            type: 'post',
            data: round,
            success: function (res) {
                GetAllIPDRounds();
                $("#RoundModal").modal('hide');
            }
        });
    }

    const onClickDelete = (id) => {
        DeleteFnc(() => onDelete(id));
    }

    const onDelete = (id) => {
        $.ajax({
            url: `/ipdrecords/DeleteRound/${id}`,
            type: 'get',
            success: function (res) {
                DeleteAlert('center', 'success', 'Round');
                GetAllIPDRounds();
            }
        });
    }
    return (
        <div className="row">
            <div className="col-xs-12">
                <button type="button" className="btn btn-click btn-primary gradient-blue pull-right form-group" data-toggle="modal" data-target="#RoundModal">Add Round</button>
                <table id="example" className="table vm table-small-font no-mb table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Doctor</th>
                            <th>Round Fee</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            rounds.map((round, index) =>
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{round.doctorName}</td>
                                    <td>{round.isFOC ? "FOC" : round.fee}</td>
                                    <td>
                                        <a onClick={() => onClickEdit(round.id)} className="btn btn-xs btn-secondary" style={{ marginRight: 3 }}>Edit</a>
                                        <a onClick={() => onClickDelete(round.id)} className="btn btn-xs btn-secondary">Delete</a>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>
            <div className="modal fade" id="RoundModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-drug pull-left">{editId > -1 ? 'Edit' : 'Add'} Round</h5>
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
                                                <strong>Success:</strong> Round added successfully.</div>
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
                                        <label className="form-label" for="Name">Date</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input type="date" className="form-control" onChange={(e) => setState('date', e.target.value)} value={moment(round.date).format('yyyy-MM-DD')} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Doctor</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <Select
                                                isClearable={true}
                                                value={(!!round.doctorId && round.doctorId > 0) ? doctors.map(x => ({ label: x.name, value: x.id })).find(x => x.value == round.doctorId) : null}
                                                onChange={(e) => { setState('doctorId', e ? e.value : null) }}
                                                options={doctors.map(x => ({ label: x.name, value: x.id }))}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <input id="round_isFOC" tabindex="5" type="checkbox" className="skin-square-grey" onChange={e => setState('isFOC', e.target.checked)} checked={round.isFOC} value={round.isFOC} />
                                        <label className="icheck-label form-label" for="round_isFOC" style={{ paddingLeft: 5 }}>FOC</label>
                                    </div>
                                    {
                                        !round.isFOC &&
                                        <div className="col-xs-12 form-group">
                                            <label className="form-label" for="Name">Round Fee</label>
                                            <span className="desc">*</span>
                                            <div className="controls">
                                                <input type="number" className="form-control" onChange={(e) => setState('fee', e.target.value)} value={round.fee} />
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
            <Round {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('round_root')
ReactDOM.render(<App ref={component => window.roundComponent = component} />, rootElement)