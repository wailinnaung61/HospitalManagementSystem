const { Component, useState, useEffect, useRef, Fragment } = React;

const Food = (props) => {
    const [orderFoods, setOrderFoods] = useState([]);
    const [foods, setFoods] = useState([]);
    const [food, setFood] = useState({
        id: '',
        date: _selectedDate,
        iPDRecordId: _iPDRecordId,
        foodId: '',
        unitPrice: '',
        qty: '',
        isFOC: false,
        sortOrder: ''
    });
    const [editId, setEditId] = useState(-1);
    const [status, setStatus] = useState('');
    const [error, setError] = useState('');

    const GetAllIPDFoods = () => {
        $.ajax({
            url: `/ipdrecords/GetIPDFoods?id=${_iPDRecordId}&date=${_selectedDate}`,
            type: 'get',
            success: function (res) {
                setOrderFoods(JSON.parse(JSON.stringify(res)));
            }
        });
    }

    useEffect(() => {
        $('#FoodModal').on('hidden.bs.modal', function (e) {
            clearForm();
            setStatus('');
            setError('');
        })
        GetAllIPDFoods();
        $.ajax({
            url: `/foods/GetAll`,
            type: 'get',
            success: function (res) {
                setFoods(JSON.parse(JSON.stringify(res || [])));
            }
        });
    }, [])

    useEffect(() => {
        console.log(food)
    }, [food])

    const clearForm = () => {
        setFood(JSON.parse(JSON.stringify({
            id: '',
            date: _selectedDate,
            iPDRecordId: _iPDRecordId,
            foodId: '',
            unitPrice: '',
            qty: '',
            isFOC: false,
            sortOrder: ''
        })));
        setEditId(-1);
    }

    const setState = (key, value) => {
        if (key == "foodId" && value) {
            var selectedFood = foods.find(x => x.id == value);
            console.log(foods, value, selectedFood)
            if (selectedFood) {
                food["unitPrice"] = food["isFOC"] ? 0 : selectedFood.unitPrice;
                if (!food["qty"]) food["qty"] = 1;
            }
        } else if (key == "isFOC") {
            if (value) {
                food["unitPrice"] = 0;
            }
        }
        food[key] = value;
        setFood(JSON.parse(JSON.stringify(food)));
    }

    const onSave = async () => {
        await setStatus('');
        await setError('');
        if (food && food.foodId && food.qty && (food.isFOC || food.unitPrice > 0)) {
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
            url: `/ipdrecords/AddFood`,
            type: 'post',
            data: food,
            success: function (res) {
                // orderFoods.push(food);
                // setOrderFoods(JSON.parse(JSON.stringify(res)));
                GetAllIPDFoods();
                setStatus('success');
                clearForm();
            }
        });
    }

    const onClickEdit = (id) => {
        setFood(JSON.parse(JSON.stringify(orderFoods.find(x => x.id == id))));
        setEditId(id);
        $("#FoodModal").modal('show');
    }

    const onEdit = () => {
        $.ajax({
            url: `/ipdrecords/UpdateFood`,
            type: 'post',
            data: food,
            success: function (res) {
                GetAllIPDFoods();
                $("#FoodModal").modal('hide');
            }
        });
    }

    const onClickDelete = (id) => {
        DeleteFnc(() => onDelete(id));
    }

    const onDelete = (id) => {
        $.ajax({
            url: `/ipdrecords/DeleteFood/${id}`,
            type: 'get',
            success: function (res) {
                DeleteAlert('center', 'success', 'Food');
                GetAllIPDFoods();
            }
        });
    }
    return (
        <div className="row">
            <div className="col-xs-12">
                <button type="button" className="btn btn-click btn-primary gradient-blue pull-right form-group" data-toggle="modal" data-target="#FoodModal">Add Food</button>
                <table id="example" className="table vm table-small-font no-mb table-bordered table-striped">
                    <thead>
                        <tr>
                            <th>No</th>
                            <th>Food</th>
                            <th>Unit Price</th>
                            <th>Qty</th>
                            <th>Amount</th>
                            <th></th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            orderFoods.map((food, index) =>
                                <tr>
                                    <td>{index + 1}</td>
                                    <td>{food.foodName}</td>
                                    <td>{food.isFOC ? "FOC" : food.unitPrice}</td>
                                    <td>{food.qty}</td>
                                    <td>{food.unitPrice * food.qty}</td>
                                    <td>
                                        <a onClick={() => onClickEdit(food.id)} className="btn btn-xs btn-secondary" style={{ marginRight: 3 }}>Edit</a>
                                        <a onClick={() => onClickDelete(food.id)} className="btn btn-xs btn-secondary">Delete</a>
                                    </td>
                                </tr>
                            )
                        }
                    </tbody>
                </table>
            </div>
            <div className="modal fade" id="FoodModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
                <div className="modal-dialog" role="document">
                    <div className="modal-content">
                        <div className="modal-header">
                            <h5 className="modal-drug pull-left">{editId > -1 ? 'Edit' : 'Add'} Food</h5>
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
                                                <strong>Success:</strong> Food added successfully.</div>
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
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Date</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input type="date" className="form-control" onChange={(e) => setState('date', e.target.value)} value={moment(food.date).format('yyyy-MM-DD')} />
                                        </div>
                                    </div>
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Food</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <Select
                                                isClearable={true}
                                                value={(!!food.foodId && food.foodId > 0) ? foods.map(x => ({ label: x.name, value: x.id })).find(x => x.value == food.foodId) : null}
                                                onChange={(e) => { setState('foodId', e ? e.value : null) }}
                                                options={foods.map(x => ({ label: x.name, value: x.id }))}
                                            />
                                        </div>
                                    </div>
                                    <div className="col-xs-12 form-group">
                                        <div className="">
                                            <input id="food_isFOC" tabindex="5" type="checkbox" className="skin-square-grey" onChange={e => setState('isFOC', e.target.checked)} checked={food.isFOC} value={food.isFOC} />
                                            <label className="icheck-label form-label" for="food_isFOC" style={{ paddingLeft: 5 }}>FOC</label>
                                        </div>
                                    </div>
                                    {
                                        !food.isFOC &&
                                        <div className="col-xs-6 form-group">
                                            <label className="form-label" for="Name">Unit Price</label>
                                            <span className="desc">*</span>
                                            <div className="controls">
                                                <input type="number" className="form-control" onChange={(e) => setState('unitPrice', e.target.value)} value={food.unitPrice} />
                                            </div>
                                        </div>
                                    }
                                    <div className="col-xs-6 form-group">
                                        <label className="form-label" for="Name">Qty</label>
                                        <span className="desc">*</span>
                                        <div className="controls">
                                            <input type="number" className="form-control" onChange={(e) => setState('qty', e.target.value)} value={food.qty} />
                                        </div>
                                    </div>
                                    {
                                        !food.isFOC &&
                                        <div className="col-xs-12 form-group">
                                            <label className="form-label" for="Name">Amount</label>
                                            <span className="desc"></span>
                                            <div className="controls">
                                                <input disabled className="form-control" value={food.unitPrice * food.qty} />
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
            <Food {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('food_root')
ReactDOM.render(<App ref={component => window.foodComponent = component} />, rootElement)