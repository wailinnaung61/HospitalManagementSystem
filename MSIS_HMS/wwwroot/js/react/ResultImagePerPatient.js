const { Component, useState, useEffect, useRef, Fragment } = React;
$(document).ready(function () {

});
const ResultImagePerPatient = (props) => {
    const [Images, setImages] = useState([]);
    const [Name, setName] = useState('');
    const [file, setFile] = useState([]);
    const [formDatas, setFormData] = useState([]);
    const [AttachmentPath, setPath] = useState([]);
    const [error, setError] = useState(_error);

    useEffect(() => {


    }, []);
    function handleChange(event) {
        setFile(event.target.files[0])
        console.log("file", file);
    }
    const onAddItem = () => {
        const fdata = new FormData();
        fdata.append(
            'file', file
        );
        
        $.ajax({
            url: `/Images/UploadFilesAjax`,/*?fileName=${ formData }*/
            type: `POST`,
            data: fdata,
            processData: false,
            contentType: false,
            success: function (res) {
                // setItems(JSON.parse(JSON.stringify(res || [])));
                console.log("res", res);
               // AttachmentPath.push(res);
                setPath(JSON.parse(JSON.stringify(res || [])));
                console.log("Path", AttachmentPath);

                var formDataLst = {
                    name: Name,
                    file: file,
                    fileName: file.name,
                    AttachmentPath: res
                }

                formDatas.push(formDataLst);
                setFormData(JSON.parse(JSON.stringify(formDatas)));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
    
       

    }
    const onDeleteItem = (index) => {
        console.log("FormDatas", formDatas);
        arrayRemoveByIndex(formDatas, index);
        setFormData(JSON.parse(JSON.stringify(formDatas)));
    }
    return (
        <div className="col-md-12 form-group p-50">

            <div className="col-md-12 form-group">
                {
                    error && <span className="text-danger mb-5">{error}</span>
                }
                <div className="row">
                    <div className="col-md-4 form-group">
                        <label class="form-label" for="Name">Name</label>
                        <span class="desc"></span>
                        <div class="controls">
                            <input type="text" className="form-control" onChange={(e) => setName(e.target.value)} value={Name} />
                            <span asp-validation-for="Name" className="text-danger"></span>
                        </div>
                    </div>
                    <div className="col-md-4 form-group">
                       {/* <span className="desc">*</span>*/}
                        <input type="file" id="fileinput" name="file-input" className="form-control-file" onChange={handleChange} style={{ marginTop: "33px" }}/>
                    </div>
                    <div className="col-md-4 form-group">
                        <button type="button" onClick={onAddItem} className="btn btn-primary" style={{ marginTop: "33px" }}><i className="fa fa-plus"></i></button>
                    </div>
                </div>
                <table id="example" className="display table table-hover table-condensed">
                    <thead>
                        <tr>
                            <th>No.</th>
                            <th>Name</th>
                            <th>File Name</th>
                            <th></th>

                        </tr>
                    </thead>
                    <tbody>
                        {
                            sort(formDatas, "sortOrder").map((formData, index) => {
                                return (
                                    <tr key={index}>
                                        <td>
                                            <input name={`${_itemType}[${index}].SortOrder`} value={index + 1} hidden />
                                            {index + 1}
                                        </td>
                                        <td>
                                          
                                            {formData.name || (formData.name ? formData.name : '')}
                                        </td>
                                        <td>
                                            <input name={`Name`} value={formData.fileName} hidden />
                                            {formData.fileName}
                                        </td>
                                        <td>
                                            <input name={`AttachmentPath`} value={formData.AttachmentPath} hidden />
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
        </div>
    );
}

class App extends Component {
    constructor(props) {
        super(props);

        this.state = {}

        this.clear = this.clear.bind(this);
    }

    componentWillUnmount() {
        this.clear();
    }

    clear = () => this.setState({});

    render() {
        return (
            <ResultImagePerPatient {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)
