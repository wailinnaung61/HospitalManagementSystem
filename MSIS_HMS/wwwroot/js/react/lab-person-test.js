const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const LabPerson_LabTestForm = (props) => {
    const [labPerson_LabTests, setLabPerson_LabTests] = useState(_labPerson_LabTests);
    const [feeTypes, setFeeTypes] = useState(_feeTypes.map(x => ({ value: x.value, label: x.text })));
    const [labTests, setLabTests] = useState([]);
    const [labPerson_LabTest, setLabPerson_LabTest] = useState({
        id: '',
        labTestName: '',
        labTestId: null,
        fee: '',
        feeType: null,
        sortOrder: '',
    });
    const [error, setError] = useState(_error);

    useEffect(() => {
        $.ajax({
            url: `/LabTests/GetAll`,
            type: 'get',
            success: function (res) {
                setLabTests(JSON.parse(JSON.stringify(res.map(x => ({ value: x.id, label: x.name })) || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
    }, []);

    const setState = (key, value) => {
        labPerson_LabTest[key] = value;
        setLabPerson_LabTest(JSON.parse(JSON.stringify(labPerson_LabTest)));
    }

    const onAddItem = () => {
        if (labPerson_LabTest.labTestId && labPerson_LabTest.fee && labPerson_LabTest.feeType) {
            setError('');
            var index = labPerson_LabTests.findIndex(x => x.labTestId == labPerson_LabTest.labTestId);
            if (index < 0) { // New
                labPerson_LabTests.push(labPerson_LabTest);
                setLabPerson_LabTests(JSON.parse(JSON.stringify(labPerson_LabTests)));
                reset();
            } else { // Exist
                Swal.fire({
                    title: 'Already Exist',
                    text: "Do you want to replace?",
                    icon: 'warning',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Yes, replace it!'
                }).then((result) => {
                    if (result.isConfirmed) {
                        labPerson_LabTests[index].fee = labPerson_LabTest.fee;
                        labPerson_LabTests[index].feeType = labPerson_LabTest.feeType;
                        setLabPerson_LabTests(JSON.parse(JSON.stringify(labPerson_LabTests)));
                        reset();
                        Swal.fire(
                            'Replaced!',
                            'It has been replaced.',
                            'success'
                        )
                    }
                })
            }
        } else {
            setError('* fields are required');
        }
    }

    const reset = () => {
        setLabPerson_LabTest(JSON.parse(JSON.stringify({
            id: '',
            labTestId: null,
            fee: '',
            feeType: null,
            sortOrder: '',
        })));
    }

    const onDeleteItem = (index) => {
        arrayRemoveByIndex(labPerson_LabTests, index);
        setLabPerson_LabTests(JSON.parse(JSON.stringify(labPerson_LabTests)));
    }

    const changeArrPosition = (arr, fromIndex, toIndex) => {
        const element = arr.splice(fromIndex, 1)[0];
        arr.splice(toIndex, 0, element);
        return arr;
    }

    const onSortingUp = (index) => {
        if (index > 0) {
            var arr = changeArrPosition(labPerson_LabTests, index, index - 1);
            setLabPerson_LabTests(JSON.parse(JSON.stringify(arr)));
        }
    }

    const onSortingDown = (index) => {
        if (index < labPerson_LabTests.length - 1) {
            var arr = changeArrPosition(labPerson_LabTests, index, index + 1);
            setLabPerson_LabTests(JSON.parse(JSON.stringify(arr)));
        }
    }

    return (
        <div className="col-md-12 form-group p-50">
            <div className='row'>
                <div className="col-md-12">
                    <h2 className="form-title pull-left">Tests</h2>
                    <div className="actions panel_actions pull-right">
                    </div>
                </div>
                <div className="col-md-12">
                    <div className="row">
                        <div className="col-md-4 form-group">
                            <label className="form-label" for="ItemId">Test</label>
                            <span className="desc">*</span>
                            <div className="controls">
                                <Select
                                    value={labPerson_LabTest.labTestId ? labTests.find(x => x.value == labPerson_LabTest.labTestId) : null}
                                    onChange={(e) => { setState('labTestId', e.value); setState('labTestName', e.label); }}
                                    options={labTests}
                                />
                            </div>
                        </div>
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="Fee">Fee</label>
                            <span className="desc"></span>
                            <div className="controls">
                                <input type="number" id="Fee" className="form-control" inputmode="numeric" pattern="[0-9]*" onChange={(e) => setState('fee', e.target.value)} value={labPerson_LabTest.fee} />
                            </div>
                        </div>
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="FeeType">Type</label>
                            <span className="desc">*</span>
                            <div className="controls">
                                <Select
                                    value={labPerson_LabTest.feeType ? feeTypes.find(x => x.value == labPerson_LabTest.feeType) : null}
                                    onChange={(e) => setState('feeType', e.value)}
                                    options={feeTypes}
                                />
                            </div>
                        </div>
                        <div className="col-md-1 form-group">
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
                    <table className="display table table-hover table-condensed">
                        <thead>
                            <tr>
                                <th width="30%">Test</th>
                                <th width="15%">Fee</th>
                                <th width="15%">Fee Type</th>
                                <th width="10%">Sorting</th>
                                <th width="10%" style={{ textAlign: 'center' }}></th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                sort(labPerson_LabTests, "sortOrder").map((detail, index) => {
                                    var name = `LabPerson_LabTests[${index}].`;
                                    return (
                                        <React.Fragment>
                                            <tr>
                                                {
                                                    detail.isTitle ?
                                                        <td colSpan={4}><b>{detail.name}</b></td> :
                                                        <React.Fragment>
                                                            <td>{detail.labTestName}</td>
                                                            <td>{detail.fee}</td>
                                                            <td>{feeTypes.find(x => x.value == detail.feeType).label}</td>
                                                        </React.Fragment>
                                                }
                                                <td>
                                                    <a href="javascript:void(0);" style={{ marginRight: 10 }} onClick={() => onSortingUp(index)}><i class="fa fa-chevron-up"></i></a>
                                                    <a href="javascript:void(0);" onClick={() => onSortingDown(index)}><i class="fa fa-chevron-down"></i></a>
                                                </td>
                                                <td>
                                                    <input type="hidden" name={`${name}LabTestId`} value={detail.labTestId} />
                                                    <input type="hidden" name={`${name}Fee`} value={detail.fee} />
                                                    <input type="hidden" name={`${name}FeeType`} value={detail.feeType} />
                                                    <input type="hidden" name={`${name}SortOrder`} value={index} />
                                                    <button type="button" onClick={() => onDeleteItem(index)} className="btn btn-xs btn-secondary">Delete</button>
                                                </td>
                                            </tr>
                                        </React.Fragment>
                                    )
                                })
                            }
                        </tbody>
                    </table>
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
            <LabPerson_LabTestForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)