const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const LabTestForm = (props) => {
    const [labTestDetails, setLabTestDetails] = useState(sort(_labTestDetails, "sortOrder"));
    const [labResultTypes, setLabResultTypes] = useState(_labResultTypes.map(x => ({ value: x.value, label: x.text })));
    const [labTestDetail, setLabTestDetail] = useState({
        id: '',
        name: '',
        labTestId: '',
        labUnit: '',
        minRange: '',
        maxRange: '',
        selectList: '',
        labResultType: 0,
        isTitle: false,
        sortOrder: '',
    });
    const [error, setError] = useState(_error);

    useEffect(() => {
        // $.ajax({
        //     url: `/outlets/GetOutletItemsByUserOutlet`,
        //     type: 'get',
        //     success: function (res) {
        //         setItems(JSON.parse(JSON.stringify(res || [])));
        //         onAddItem();
        //     },
        //     error: function (jqXhr, textStatus, errorMessage) {

        //     }
        // });
    }, []);

    const setState = (key, value) => {
        labTestDetail[key] = value;
        console.log(key, value)
        if (key == "isTitle" && value) {
            labTestDetail['labResultType'] = 0;
            labTestDetail['labUnit'] = '';
            labTestDetail['selectList'] = '';
            labTestDetail['minRange'] = '';
            labTestDetail['maxRange'] = '';
        }
        setLabTestDetail(JSON.parse(JSON.stringify(labTestDetail)));
    }

    const onAddItem = () => {
        console.log(labTestDetail, labTestDetail.name && (labTestDetail.isTitle || (labTestDetail.labResultType !== '' && labTestDetail.labResultType !== null && labTestDetail.labResultType !== undefined)))
        if (labTestDetail.name && (labTestDetail.isTitle || (labTestDetail.labResultType !== '' && labTestDetail.labResultType !== null && labTestDetail.labResultType !== undefined))) {
            setError('');
            labTestDetail['sortOrder'] = labTestDetails.length;
            labTestDetails.push(labTestDetail);
            setLabTestDetails(JSON.parse(JSON.stringify(labTestDetails)));
            reset();
        } else {
            setError('* fields are required');
        }
    }

    const reset = () => {
        setLabTestDetail(JSON.parse(JSON.stringify({
            id: '',
            name: '',
            labTestId: '',
            labUnit: '',
            minRange: '',
            maxRange: '',
            selectList: '',
            labResultType: 0,
            isTitle: false,
            sortOrder: '',
        })));
    }

    const onDeleteItem = (index) => {
        arrayRemoveByIndex(labTestDetails, index);
        setLabTestDetails(JSON.parse(JSON.stringify(labTestDetails)));
    }

    const changeArrPosition = (arr, fromIndex, toIndex) => {
        const element = arr.splice(fromIndex, 1)[0];
        arr.splice(toIndex, 0, element);
        return arr;
    }

    const onSortingUp = (index) => {
        debugger;
        if (index > 0) {
            var arr = changeArrPosition(labTestDetails, index, index - 1);
            setLabTestDetails(JSON.parse(JSON.stringify(arr)));
        }
    }

    const onSortingDown = (index) => {
        if (index < labTestDetails.length - 1) {
            var arr = changeArrPosition(labTestDetails, index, index + 1);
            setLabTestDetails(JSON.parse(JSON.stringify(arr)));
        }
    }

    return (
        <div className="col-md-12 form-group p-50">
            <div className='row'>
                <div className="col-md-12">
                    <h2 className="form-title pull-left">Lab Details</h2>
                    <div className="actions panel_actions pull-right">
                    </div>
                </div>
                <div className="col-md-12">
                    <div className="row">
                        <div className="col-md-4 form-group">
                            <label className="form-label" for="ItemId">Name</label>
                            <span className="desc">*</span>
                            <div className="controls">
                                <input type="text" id="Name" className="form-control" onChange={(e) => setState('name', e.target.value)} value={labTestDetail.name} />
                            </div>
                        </div>
                        <div className="col-md-1 form-group">
                            <label className="form-label" for="IsTitle">Title</label>
                            <span className="desc"></span>
                            <div className="controls">
                                <input type="checkbox" onClick={(e) => setState('isTitle', e.target.checked)} value={labTestDetail.isTitle} checked={labTestDetail.isTitle} className="skin-square-grey" id="IsTitle" />
                            </div>
                        </div>
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="LabResultType">Type</label>
                            <span className="desc">*</span>
                            <div className="controls">
                                <Select
                                    value={labResultTypes.find(x => x.value == labTestDetail.labResultType)}
                                    onChange={(e) => setState('labResultType', e.value)}
                                    options={labResultTypes}
                                    isDisabled={labTestDetail.isTitle}
                                />
                            </div>
                        </div>
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="LabUnit">Unit</label>
                            <span className="desc"></span>
                            <div className="controls">
                                <input type="text" id="SelectList" className="form-control" disabled={labTestDetail.isTitle} onChange={(e) => setState('labUnit', e.target.value)} value={labTestDetail.labUnit} />
                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="LabUnit">Min Range</label>
                            <span className="desc"></span>
                            <div className="controls">
                                <input type="number" id="MinRange" className="form-control" inputmode="numeric" pattern="[0-9]*" disabled={labTestDetail.labResultType != 1} onChange={(e) => setState('minRange', e.target.value)} value={labTestDetail.minRange} />
                            </div>
                        </div>
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="LabUnit">Max Range</label>
                            <span className="desc"></span>
                            <div className="controls">
                                <input type="number" id="MaxRange" className="form-control" inputmode="numeric" pattern="[0-9]*" disabled={labTestDetail.labResultType != 1} onChange={(e) => setState('maxRange', e.target.value)} value={labTestDetail.maxRange} />
                            </div>
                        </div>

                        <div className="col-md-5 form-group">
                            <label className="form-label" for="LabUnit">Select List</label>
                            <span className="desc">(eg. Positive/Negative/Neutral)</span>
                            <div className="controls">
                                <input type="text" id="LabUnit" className="form-control" disabled={labTestDetail.labResultType != 2} onChange={(e) => setState('selectList', e.target.value)} value={labTestDetail.selectList} />
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
                                <th width="30%">Name</th>
                                <th width="15%">Type</th>
                                <th width="15%">Unit</th>
                                <th width="25%">Reference</th>
                                <th width="10%">Sorting</th>
                                <th width="10%" style={{ textAlign: 'center' }}></th>
                            </tr>
                        </thead>
                        <tbody>
                            {
                                labTestDetails.map((detail, index) => {
                                    var name = `LabTestDetails[${index}].`;
                                    return (
                                        <React.Fragment>
                                            <tr>
                                                {
                                                    detail.isTitle ?
                                                        <td colSpan={4}><b>{detail.name}</b></td> :
                                                        <React.Fragment>
                                                            <td>{detail.name}</td>
                                                            <td>{labResultTypes.find(x => x.value == detail.labResultType).label}</td>
                                                            <td>{detail.labUnit}</td>
                                                            <td>{detail.labResultType == 1 ? `${detail.minRange}~${detail.maxRange}` : detail.labResultType == 2 ? detail.selectList : ''}</td>
                                                        </React.Fragment>
                                                }
                                                <td>
                                                    <a href="javascript:void(0);" style={{ marginRight: 10 }} onClick={() => onSortingUp(index)}><i class="fa fa-chevron-up"></i></a>
                                                    <a href="javascript:void(0);" onClick={() => onSortingDown(index)}><i class="fa fa-chevron-down"></i></a>
                                                </td>
                                                <td>
                                                    <input type="hidden" name={`${name}Name`} value={detail.name} />
                                                    <input type="hidden" name={`${name}IsTitle`} value={detail.isTitle} />
                                                    <input type="hidden" name={`${name}LabUnit`} value={detail.labUnit} />
                                                    <input type="hidden" name={`${name}MinRange`} value={detail.minRange} />
                                                    <input type="hidden" name={`${name}MaxRange`} value={detail.maxRange} />
                                                    <input type="hidden" name={`${name}SelectList`} value={detail.selectList} />
                                                    <input type="hidden" name={`${name}LabResultType`} value={detail.labResultType} />
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
            <LabTestForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)