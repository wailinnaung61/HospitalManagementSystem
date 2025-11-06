const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const TransferItemForm = (props) => {
    const [schedules, setSchedules] = useState(_schedules);
    const [departments, setDepartments] = useState([]);
    const [daysOfWeek, setDaysOfWeek] = useState([]);
    const [department, setDepartment] = useState(null);
    const [dayOfWeek, setDayOfWeek] = useState(null);
    const [fromTime, setFromTime] = useState('');
    const [toTime, setToTime] = useState('');
    const [error, setError] = useState(_error);

    useEffect(() => {
        $.ajax({
            url: `/Departments/GetAll`,
            type: 'get',
            success: function (res) {
                setDepartments(JSON.parse(JSON.stringify(res || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
        $.ajax({
            url: `/Settings/GetDaysOfWeekSelectList`,
            type: 'get',
            success: function (res) {
                setDaysOfWeek(JSON.parse(JSON.stringify(res || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });

        return () => {
            setSchedules(_schedules);
            setDepartments([]);
            setDaysOfWeek([]);
            setDepartment(undefined);
            setDayOfWeek(undefined);
            setFromTime(undefined);
            setToTime(undefined);
            setError(_error);
        }
    }, []);

    const onAddItem = () => {
        //console.log("onAddItem");
        if (department && dayOfWeek && fromTime && toTime) {
            setError('');
            var schedule = {
                departmentId: department.value,
                dayOfWeek: dayOfWeek.value,
                departmentName: department.label,
                dayOfWeekName: dayOfWeek.label,
                fromTime,
                toTime
            };
            schedules.push(schedule);
            setSchedules(JSON.parse(JSON.stringify(schedules)));
        } else {
            setError('* fields are required');
        }
    }

    useDidUpdateEffect(() => {
        setDepartment(null);
        setDayOfWeek(null);
        setFromTime('');
        setToTime('');
    }, [schedules])

    const onDeleteItem = (index) => {
        arrayRemoveByIndex(schedules, index);
        setSchedules(JSON.parse(JSON.stringify(schedules)));
    }

    return (
        <div className="col-md-12 form-group p-50">
            <div className="col-md-12">
                <h2 className="form-title pull-left">Schedules</h2>
                <div className="actions panel_actions pull-right">
                </div>
            </div>
            <div className="col-md-12">
                <div className="row">
                    <div className="col-md-3 form-group">
                        <label className="form-label" for="ItemId">Department</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <Select
                                value={department}
                                onChange={(e) => setDepartment(e)}
                                options={departments.map(x => ({ value: x.id, label: x.name }))}
                            />
                        </div>
                    </div>
                    <div className="col-md-3 form-group">
                        <label className="form-label" for="UnitId">Day of Week</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <Select
                                value={dayOfWeek}
                                onChange={(e) => setDayOfWeek(e)}
                                options={daysOfWeek}
                            />
                        </div>
                    </div>
                    <div className="col-md-2 form-group">
                        <label className="form-label" for="Qty">From</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <input type="time" className="form-control" onChange={(e) => setFromTime(e.target.value)} value={fromTime} />
                        </div>
                    </div>
                    <div className="col-md-2 form-group">
                        <label className="form-label" for="Qty">To</label>
                        <span className="desc">*</span>
                        <div className="controls">
                            <input type="time" className="form-control" onChange={(e) => setToTime(e.target.value)} value={toTime} />
                        </div>
                    </div>
                    <div className="col-md-2 form-group">
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
                            <th>Department</th>
                            <th>DayOfWeek</th>
                            <th>Batch</th>
                            <th>Qty</th>
                            <th style={{ textAlign: 'center' }}></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            sort(schedules, "departmentId").map((schedule, index) => {
                                return (
                                    <tr key={index}>
                                        <td>
                                            <input name={`Schedules[${index}].SortOrder`} value={index + 1} hidden />
                                            {index + 1}
                                        </td>
                                        <td>
                                            <input name={`Schedules[${index}].DepartmentId`} value={schedule.departmentId} hidden />
                                            <input name={`Schedules[${index}].DepartmentName`} value={schedule.departmentName} hidden />
                                            {schedule.departmentName}
                                        </td>
                                        <td>
                                            <input name={`Schedules[${index}].DayOfWeek`} value={schedule.dayOfWeek} hidden />
                                            <input name={`Schedules[${index}].DayOfWeekName`} value={schedule.dayOfWeekName} hidden />
                                            {schedule.dayOfWeekName}
                                        </td>
                                        <td>
                                            <input name={`Schedules[${index}].FromTime`} value={schedule.fromTime} hidden />
                                            {tConvert(schedule.fromTime)}
                                        </td>
                                        <td>
                                            <input name={`Schedules[${index}].ToTime`} value={schedule.toTime} hidden />
                                            {tConvert(schedule.toTime)}
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
        }

        this.clear = this.clear.bind(this);
    }

    componentWillUnmount() {
        this.clear();
    }

    clear = () => this.setState({});

    render() {
        return (
            <TransferItemForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)