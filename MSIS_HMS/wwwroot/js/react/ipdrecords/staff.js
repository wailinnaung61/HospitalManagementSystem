const { Component, useState, useEffect, useRef, Fragment } = React;

const Staff = (props) => {
    const [staffs, setStaffs] = useState([]);
    const [staffList, setStaffList] = useState([]);
    const [staff, setStaff] = useState({
        id: '',
        date: _selectedDate,
        iPDRecordId: _iPDRecordId,
        staffId: '',
        fee: '',
        isFOC: false,
        sortOrder: ''
    });
    const [editId, setEditId] = useState(-1);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');

    const GetAllIPDStaffs = () => {
        $.ajax({
            url: `/ipdrecords/GetIPDStaffs?id=${_iPDRecordId}&date=${_selectedDate}`,
            type: 'get',
            success: function (res) {
                setStaffs(JSON.parse(JSON.stringify(res)));
            }
        });
    }

    useEffect(() => {
        $('#StaffModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })
        GetAllIPDStaffs();
        $.ajax({
            url: `/staffs/GetAll?DepartmentType=2`,
            type: 'get',
            success: function (res) {
                setStaffList(JSON.parse(JSON.stringify(res || [])));
            }
        });
    }, [])

    useEffect(() => {
        console.log(staff)
    }, [staff])

    const clearForm = () => {
        setStaff(JSON.parse(JSON.stringify({
            id: '',
            date: _selectedDate,
            iPDRecordId: _iPDRecordId,
            staffId: '',
            fee: '',
            isFOC: false,
            sortOrder: ''
        })));
        setEditId(-1);
    }

    const setState = (key, value) => {
        if (key == "staffId") {
            if (value) {
                var selectedStaff = staffList.find(x => x.id == value);
                console.log(staffList, value, selectedStaff)
                if (selectedStaff) {
                    staff["fee"] = staff["isFOC"] ? 0 : (selectedStaff.staffFee || '');
                }
            } else {
                staff["fee"] = '';
            }
        } else if (key == "isFOC") {
            if (value) {
                staff["fee"] = 0;
            }
        }
        staff[key] = value;
        setStaff(JSON.parse(JSON.stringify(staff)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (staff && staff.staffId && (staff.isFOC || staff.fee > 0)) {
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
            url: `/ipdrecords/AddStaff`,
            type: 'post',
            data: staff,
            success: function (res) {
                // staffs.push(staff);
                // setStaffs(JSON.parse(JSON.stringify(res)));
                GetAllIPDStaffs();
                setStatus('success');
                clearForm();
            }
        });
    }

    const onClickEdit = (id) => {
        setStaff(JSON.parse(JSON.stringify(staffs.find(x => x.id == id))));
        setEditId(id);
        $("#StaffModal").modal('show');
    }

    const onEdit = () => {
        $.ajax({
            url: `/ipdrecords/UpdateStaff`,
            type: 'post',
            data: staff,
            success: function (res) {
                GetAllIPDStaffs();
                $("#StaffModal").modal('hide');
            }
        });
    }

    const onClickDelete = (id) => {
        DeleteFnc(() => onDelete(id));
    }

    const onDelete = (id) => {
        $.ajax({
            url: `/ipdrecords/DeleteStaff/${id}`,
            type: 'get',
            success: function (res) {
                DeleteAlert('center', 'success', 'Staff');
                GetAllIPDStaffs();
            }
        });
    }
    return (
        <div className="row">
            <div className="col-xs-12">
                <button type="button" className="btn btn-click btn-primary gradient-blue pull-right form-group" data-toggle="modal" data-target="#StaffModal">Add Staff</button>
                <table id="example" className="table vm table-small-font no-mb table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Staff</th>
                            <th>Fee</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            staffs.map((staff, index) =>
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{staff.staffName}</td>
                                    <td>{staff.isFOC ? "FOC" : staff.fee}</td>
                                    <td>
                                        <a onClick={() => onClickEdit(staff.id)} className="btn btn-xs btn-secondary" style={{ marginRight: 3 }}>Edit</a>
                                        <a onClick={() => onClickDelete(staff.id)} className="btn btn-xs btn-secondary">Delete</a>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>
            <div className="modal fade" id="StaffModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-drug pull-left">{editId > -1 ? 'Edit' : 'Add'} Staff</h5>
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
                                                <strong>Success:</strong> Staff added successfully.</div>
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
                                            <input type="date" className="form-control" onChange={(e) => setState('date', e.target.value)} value={moment(staff.date).format('yyyy-MM-DD')} />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <label className="form-label" for="Name">Staff</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <Select
                                                isClearable={true}
                                                value={(!!staff.staffId && staff.staffId > 0) ? staffList.map(x => ({ label: x.name, value: x.id })).find(x => x.value == staff.staffId) : null}
                                                onChange={(e) => { setState('staffId', e ? e.value : null) }}
                                                options={staffList.map(x => ({ label: x.name, value: x.id }))}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <input id="staff_isFOC" tabindex="5" type="checkbox" className="skin-square-grey" onChange={e => setState('isFOC', e.target.checked)} checked={staff.isFOC} value={staff.isFOC} />
                                        <label className="icheck-label form-label" for="staff_isFOC" style={{ paddingLeft: 5 }}>FOC</label>
                                    </div>
                                    {
                                        !staff.isFOC &&
                                        <div className="col-xs-12 form-group">
                                            <label className="form-label" for="Name">Fee</label>
                                            <span className="desc">*</span>
                                            <div className="controls">
                                                <input type="number" className="form-control" onChange={(e) => setState('fee', e.target.value)} value={staff.fee} />
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
            <Staff {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('staff_root')
ReactDOM.render(<App ref={component => window.staffComponent = component} />, rootElement)