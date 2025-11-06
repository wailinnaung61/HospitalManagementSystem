
const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const LabResultForm = (props) => {
    const [labResultDetails, setLabResultDetails] = useState(_labResultDetails);
    const [labResultFiles, setLabResultFiles] = useState(_labResultFiles);
    const [labResultTypes, setLabResultTypes] = useState(_labResultTypes.map(x => ({ value: x.value, label: x.text })));
    const [labTests, setLabTests] = useState([]);
    const [technicians, setTechnicians] = useState([]);
    const [consultants, setConsultants] = useState([]);
    const [labTestId, setLabTestId] = useState(_labTestId);
    const [technicianId, setTechnicianId] = useState(_technicianId);
    const [consultantId, setConsultantId] = useState(_consultantId);
    const [labOrderId, setLabOrderId] = useState('');
    const [isLabReport, setIsLabReport] = useState(true);
    const [Name, setName] = useState('');
    const [formDatas, setFormData] = useState([]);
    const [file, setFile] = useState([]);
    const [error, setError] = useState(_error);
    const prevLabTestId = usePrevious(labTestId);
    const [labOrder, setLabOrder] = useState([]);


    useEffect(() => {
        setIsLabReport(false);
        console.log("Edit labResult ", labResultDetails);
        $.ajax({
            url: `/LabOrders/GetLabOrderFromLabOrderTest`,
            type: 'get',
            success: function (res) {
                setLabOrder(JSON.parse(JSON.stringify(res.map(x => ({ value: x.id, label: x.voucherNo })) || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
        $.ajax({
            url: `/LabTests/GetAll`,
            type: 'get',
            success: function (res) {
                setLabTests(JSON.parse(JSON.stringify(res.map(x => ({ value: x.id, label: x.name })) || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
        if (labTestId) {
            $.ajax({
                url: `/LabPersons/GetAllByLabTest?LabTestId=${labTestId}`,
                type: 'get',
                success: function (res) {
                    var labPersons = JSON.parse(JSON.stringify(res || []));
                    setTechnicians(labPersons.filter(x => x.type == 1).map(x => ({ value: x.id, label: x.name })));
                    setConsultants(labPersons.filter(x => x.type == 2).map(x => ({ value: x.id, label: x.name })));
                },
                error: function (jqXhr, textStatus, errorMessage) {
                }
            });
        }
    }, []);

    useDidUpdateEffect(() => {
        
        const onUpdateLabTest = (labTestId) => {
            setTechnicianId('');
            setConsultantId('');
            //setLabResultDetails([]);

            // if (prevLabTestId + '') Swal.showLoading();
            $.ajax({
                url: `/LabTests/GetLabResultDetails?LabTestId=${labTestId}`,
                type: 'get',
                success: function (res) {
                    if (_action == "edit") {
                        setLabResultDetails(JSON.parse(JSON.stringify(res || [])));//zpp

                    }

                    $.ajax({
                        url: `/LabPersons/GetAllByLabTest?LabTestId=${labTestId}`,
                        type: 'get',
                        success: function (res) {
                            debugger;
                            var labPersons = JSON.parse(JSON.stringify(res || []));
                            console.log("labPerson ", labPersons)


                            setTechnicians(labPersons.filter(x => x.type == 1).map(x => ({ value: x.id, label: x.name })));
                            setConsultants(labPersons.filter(x => x.type == 2).map(x => ({ value: x.id, label: x.name })));


                        },
                        error: function (jqXhr, textStatus, errorMessage) {
                            // Swal.hideLoading();
                            // Swal.fire({
                            //     icon: 'error',
                            //     title: 'Oops...',
                            //     text: 'Something went wrong while changing laboratory test!',
                            //     footer: 'Contact Administrator For the issue?'
                            // })
                        }
                    });
                },
                error: function (jqXhr, textStatus, errorMessage) {

                }
            });
        }
        // console.log(prevLabTestId + '', labTestId)
        // if (prevLabTestId + '') {
        //     Swal.fire({
        //         title: 'Previous Result will be lost!',
        //         text: "Do you want to change?",
        //         icon: 'warning',
        //         showCancelButton: true,
        //         confirmButtonColor: '#3085d6',
        //         cancelButtonColor: '#d33',
        //         confirmButtonText: 'Yes, change it!'
        //     }).then((result) => {
        //         if (result.isConfirmed) {
        //             onUpdateLabTest(labTestId);
        //         } else {

        //         }
        //     })
        // } else {
        // }
        onUpdateLabTest(labTestId);
    }, [labTestId])

    const setState = (index, key, value) => {
        labResultDetails[index][key] = value;
        setLabResultDetails(JSON.parse(JSON.stringify(labResultDetails)));
    }
    function onTestChange(event) {
        debugger;
        console.log("event", event);
        setLabTestId(event);
        $.ajax({
            url: `/LabPersons/GetAllByLabTest?LabTestId=${event}&&isImaging=${false}`,
            type: 'get',
            success: function (res) {
                debugger;
                var labPersons = JSON.parse(JSON.stringify(res || []));
                console.log("labPerson ", labPersons)
                if (labPersons[0].isLabReport == false) {


                    setIsLabReport(labPersons[0].isLabReport);

                    console.log("IsLabReport", isLabReport);
                }
                else {

                    setIsLabReport(labPersons[0].isLabReport);

                    console.log("IsLabReport", isLabReport);
                }

            },
            error: function (jqXhr, textStatus, errorMessage) {
                // Swal.hideLoading();
                // Swal.fire({
                //     icon: 'error',
                //     title: 'Oops...',
                //     text: 'Something went wrong while changing laboratory test!',
                //     footer: 'Contact Administrator For the issue?'
                // })
            }
        });
    }
    function onLabOrderChange(event) {
        console.log("event", event);
        setLabOrderId(event);
        $.ajax({
            url: `/LabTests/GetLabTestByOrderId?labOrderId=${event}`,
            type: 'get',
            success: function (res) {
                setLabTests(JSON.parse(JSON.stringify(res.map(x => ({ value: x.id, label: x.name })) || [])));

            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
              
    }
    function handleChange(event) {
        setFile(event.target.files[0])
        console.log("file", file);
    }
    const onAddItem = () => {
        debugger;
        if (isLabReport==true) {
            const fdata = new FormData();
            fdata.append(
                'file', file
            );

            $.ajax({
                url: `/Images/UploadFilesAjaxForLab`,/*?fileName=${ formData }*/
                type: `POST`,
                data: fdata,
                processData: false,
                contentType: false,
                success: function (res) {
                    // setItems(JSON.parse(JSON.stringify(res || [])));
                    var formDataLst = {

                        name: file.name,
                        attachmentPath: res
                    }

                    labResultFiles.push(formDataLst);
                    setLabResultFiles(JSON.parse(JSON.stringify(labResultFiles)));
                },
                error: function (jqXhr, textStatus, errorMessage) {

                }
            });
        }
        else {
           
              
                // if (prevLabTestId + '') Swal.showLoading();
                $.ajax({
                    url: `/LabTests/GetLabResultDetails?LabTestId=${labTestId}`,
                    type: 'get',
                    success: function (res) {
                        //setLabResultDetails(JSON.parse(JSON.stringify(res || [])));//zpp
                        for (var i = 0; i < res.length; i++) {
                            labResultDetails.push(JSON.parse(JSON.stringify(res[i] || [])));
                            labResultDetails[labResultDetails.length - 1].testId = labTestId;
                            labResultDetails[labResultDetails.length - 1].consultantId = consultantId;
                            labResultDetails[labResultDetails.length - 1].technicianId = technicianId;

                        }
                       
                        //console.log("Result List=", labResultList);
                       
                        //setLabResultDetails(JSON.parse(JSON.stringify(labResultDetails || [])));
                        $.ajax({
                            url: `/LabPersons/GetAllByLabTest?LabTestId=${labTestId}`,
                            type: 'get',
                            success: function (res) {
                                debugger;
                                var labPersons = JSON.parse(JSON.stringify(res || []));
                                console.log("labPerson ", labPersons)


                                setTechnicians(labPersons.filter(x => x.type == 1).map(x => ({ value: x.id, label: x.name })));
                                setConsultants(labPersons.filter(x => x.type == 2).map(x => ({ value: x.id, label: x.name })));
                            },
                            error: function (jqXhr, textStatus, errorMessage) {
                                // Swal.hideLoading();
                                // Swal.fire({
                                //     icon: 'error',
                                //     title: 'Oops...',
                                //     text: 'Something went wrong while changing laboratory test!',
                                //     footer: 'Contact Administrator For the issue?'
                                // })
                            }
                        });
                    },
                    error: function (jqXhr, textStatus, errorMessage) {

                    }
                });
            
        }
        console.log("LabResultDetail ", labResultDetails);
     
    }
    const onDeleteItem = (index) => {
        console.log("FormDatas", formDatas);
        arrayRemoveByIndex(labResultFiles, index);
        setLabResultFiles(JSON.parse(JSON.stringify(labResultFiles)));
    }
    return (
        <div className="col-md-12 form-group p-50">
            <div className='row'>
                <div className="col-md-12">
                    <h2 className="form-title pull-left">Result Details</h2>
                    <div className="actions panel_actions pull-right">
                     
                        <input type="hidden" name="LabTestId" value={labTestId} />
                        <input type="hidden" name="TechnicianId" value={technicianId} />
                        <input type="hidden" name="ConsultantId" value={consultantId} />
                        <input type="hidden" name="LabOrderId" value={labOrderId} />
                    </div>
                </div>
                <div className="col-md-12">
                    <div className="row">
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="LabOrderNo">LabOrderNo</label>
                            <span className="desc">*</span>
                            <div className="controls">
                                <Select
                                    //value={labOrder}
                                    //onChange={(e) => setLabTestId(e.value)}
                                    options={labOrder}
                                    isDisabled={_action == "Edit"}
                                    onChange={(e) => onLabOrderChange(e.value)}

                                />
                            </div>
                        </div>
                        <div className="col-md-3 form-group">
                            <label className="form-label" for="LabTestId">Test</label>
                            <span className="desc">*</span>
                            <div className="controls">
                                <Select
                                    value={labTests.find(x => x.value == labTestId)}
                                    //onChange={(e) => setLabTestId(e.value)}
                                    options={labTests}
                                    isDisabled={_action == "Edit"}
                                    onChange={(e) => onTestChange(e.value)}

                                />
                            </div>
                        </div>
                        {!isLabReport ?
                            <div className="col-md-3 form-group">

                                <label className="form-label" for="TechnicianId">Technician</label>
                                <span className="desc">*</span>

                                <div className="controls">
                                    <Select
                                        isClearable={true}
                                        value={technicians.find(x => x.value == technicianId)}
                                        onChange={(e) => setTechnicianId(e ? e.value : '')}
                                        isDisabled={_action=="Edit"}
                                        options={technicians}
                                    />
                                </div>
                            </div> : <div className="col-md-3 form-group">
                                <label class="form-label" for="Name">Name</label>
                                <span class="desc"></span>
                                <div class="controls">
                                    <input type="text" className="form-control" onChange={(e) => setName(e.target.value)} value={Name} />
                                    <span asp-validation-for="Name" className="text-danger"></span>
                                </div>
                            </div>
                        }
                        {!isLabReport ?
                            <div className="col-md-3 form-group">
                                <label className="form-label" for="ConsultantId">Consultant</label>
                                <span className="desc">*</span>
                                <div className="controls">
                                    <Select
                                        isClearable={true}
                                        value={consultants.find(x => x.value == consultantId)}
                                        isDisabled={_action == "Edit"}
                                        onChange={(e) => setConsultantId(e ? e.value : '')}
                                        options={consultants}
                                    />
                                </div>
                            </div> : <div className="col-md-3 form-group">
                                <input type="file" id="fileinput" name="file-input" className="form-control-file" onChange={handleChange} style={{ marginTop: "33px" }} />

                            </div>


                        }
                       {/* {isLabReport ?*/}
                            <div className="col-md-3 form-group">
                                <button type="button" onClick={onAddItem} className="btn btn-primary" style={{ marginTop: "33px" }}><i className="fa fa-plus"></i></button>
                            </div> : <div></div>
                        {/*}*/}
                    </div>
                </div>
                {!isLabReport ?

                    <div className="col-md-12 form-group">
                        {
                            error && <span className="text-danger mb-5">{error}</span>
                        }
                        <table className="display table table-hover table-condensed">
                            <thead>
                                <tr>
                                    <th width="20%">Name</th>
                                    <th width="20%">Result</th>
                                    <th width="15%">Unit</th>
                                    <th width="15%">Reference</th>
                                    <th width="30%">Remark</th>
                                </tr>
                            </thead>
                            <tbody>
                                {
                                    sort(labResultDetails, "testId").map((detail, index) => {
                                        var name = `LabResultDetails[${index}].`;
                                        return (
                                            <React.Fragment>
                                                <tr>
                                                    {
                                                        detail.isTitle ?
                                                            <td colSpan={4}><b>{detail.name}</b></td> :
                                                            <React.Fragment>
                                                                <td>{detail.name}</td>
                                                                <td>
                                                                    {
                                                                        detail.labResultType == 1 ?
                                                                            <input className="form-control" type="number" name={`${name}Result`} value={detail.result} onChange={(e) => setState(index, 'result', e.target.value)} /> :
                                                                            detail.labResultType == 2 && detail.selectList ?
                                                                                <select className="form-control" name={`${name}Result`} value={detail.result} onChange={(e) => setState(index, 'result', e.target.value)}>
                                                                                    <option value="">Please select</option>
                                                                                    {
                                                                                        detail.selectList.split('/').map(x =>
                                                                                            <option value={x}>{x}</option>
                                                                                        )
                                                                                    }
                                                                                </select> :
                                                                                <input className="form-control" type="text" name={`${name}Result`} value={detail.result} onChange={(e) => setState(index, 'result', e.target.value)} />
                                                                    }
                                                                </td>
                                                                <td>{detail.labUnit}</td>
                                                                <td>{detail.labResultType == 1 ? `${detail.minRange} ~ ${detail.maxRange}` : ''}</td>
                                                            </React.Fragment>
                                                    }
                                                    <td>
                                                        <input type="hidden" name={`${name}Name`} value={detail.name} />
                                                        <input type="hidden" name={`${name}IsTitle`} value={detail.isTitle} />
                                                        <input type="hidden" name={`${name}LabUnit`} value={detail.labUnit} />
                                                        <input type="hidden" name={`${name}MinRange`} value={detail.minRange} />
                                                        <input type="hidden" name={`${name}MaxRange`} value={detail.maxRange} />
                                                        <input type="hidden" name={`${name}SelectList`} value={detail.selectList} />
                                                        <input type="hidden" name={`${name}LabResultType`} value={detail.labResultType} />
                                                        <input type="hidden" name={`${name}TestId`} value={detail.testId} />
                                                        <input type="hidden" name={`${name}ConsultantId`} value={detail.consultantId} />
                                                        <input type="hidden" name={`${name}TechnicianId`} value={detail.technicianId} />
                                                        <input type="hidden" name={`${name}SortOrder`} value={index} />
                                                        {
                                                            !detail.isTitle && <textarea className="form-control" rows={3} name={`${name}Remark`} value={detail.remark} onChange={(e) => setState(index, 'remark', e.target.value)}></textarea>
                                                        }
                                                    </td>
                                                </tr>
                                            </React.Fragment>
                                        )
                                    })
                                }
                            </tbody>
                        </table>
                    </div> :
                    <div className="col-md-12 form-group">
                        {
                            error && <span className="text-danger mb-5">{error}</span>
                        }<table id="example" className="display table table-hover table-condensed">
                            <thead>
                                <tr>
                                    <th>No.</th>
                                    {/*<th>Name</th>*/}
                                    <th>File Name</th>
                                    <th></th>

                                </tr>
                            </thead>
                            <tbody>
                                {

                                    sort(labResultFiles, "sortOrder").map((formData, index) => {
                                        debugger;

                                        var name = `LabResultFiles[${index}].`;
                                        return (
                                            <tr key={index}>
                                                <td>
                                                    <input name={`[${index}].SortOrder`} value={index + 1} hidden />
                                                    {index + 1}
                                                </td>
                                                {/*<td>*/}

                                                {/*    {formData.name || (formData.name ? formData.name : '')}*/}
                                                {/*</td>*/}
                                                <td>
                                                    <input name={`${name}Name`} value={formData.name} hidden />
                                                    {formData.name}
                                                </td>
                                                <td>
                                                    <input name={`${name}AttachmentPath`} value={formData.attachmentPath} hidden />
                                                    {/*{formData.AttachmentPath}*/}
                                                    <button type="button" onClick={() => onDeleteItem(index)} className="btn btn-xs btn-secondary">Delete</button>
                                                </td>

                                            </tr>
                                        )
                                    })
                                }

                            </tbody>
                            <tfoot>
                                <tr>

                                    <td></td>

                                </tr>
                            </tfoot>
                        </table>
                    </div>
                }
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
            <LabResultForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)