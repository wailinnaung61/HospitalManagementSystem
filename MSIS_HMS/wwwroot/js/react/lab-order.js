const { Component, useState, useEffect, useRef, Fragment } = React;

$(document).ready(function () {

});

const OrderForm = (props) => {
    const [labTests, setLabTests] = useState([]);
    const [referrers, setReferrers] = useState([]);
    const [labResults, setLabResults] = useState([]);
    const [labOrderTests, setLabOrderTests] = useState(_labOrderTests);
    const [total, setTotal] = useState(0);
    const [tax, setTax] = useState(_tax);
    const [discount, setDiscount] = useState(_discount);
    const [error, setError] = useState(_error);

    useEffect(() => {
        $.ajax({
            url: `/labTests/GetAll`,
            type: 'get',
            success: function (res) {
                setLabTests(JSON.parse(JSON.stringify(res || [])));
                onAddLabTest();
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });

        $.ajax({
            url: `/referrers/GetAll`,
            type: 'get',
            success: function (res) {
                setReferrers(JSON.parse(JSON.stringify(res || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
        $.ajax({
            url: `/LabResults/GetAll`,
            type: 'get',
            success: function (res) {
                setLabResults(JSON.parse(JSON.stringify(res || [])));
            },
            error: function (jqXhr, textStatus, errorMessage) {

            }
        });
    }, []);

    useEffect(() => {
        console.log(labResults.map(x => ({ value: x.id, label: `${x.resultNo}` })).find(x => x.value == 1))
    }, [labResults])

    useEffect(() => {
        setTotal(calculateTotal(labOrderTests.map(x => ({ amount: x.qty * x.unitPrice })), "amount"));
    }, [labOrderTests])

    const onAddLabTest = () => {
        var labOrderTest = {
            labTestId: '',
            referrerId: '',
            labResultId: '',
            qty: 1,
            feeType: 0,
            fee: 0,
            referralFee: 0,
            unitPrice: 0,
        };
        labOrderTests.push(labOrderTest);
        setLabOrderTests(JSON.parse(JSON.stringify(labOrderTests)));
    }

    const onUpdateLabTest = (index, key, value) => {
        console.log(index, key, value)
        labOrderTests[index][key] = value;
        setLabOrderTests(JSON.parse(JSON.stringify(labOrderTests)));
    }

    const onDeleteLabTest = (index) => {
        arrayRemoveByIndex(labOrderTests, index);
        setLabOrderTests(JSON.parse(JSON.stringify(labOrderTests)));
    }

    return (
        <div className="col-md-12 form-group">
            {
                error && <span className="text-danger mb-5">{error}</span>
            }
            <table className="display table table-condensed">
                <thead>
                    <tr>
                        <th>No.</th>
                        <th width="25%">Test</th>
                        <th width="25%">Referrer</th>
                        <th width="20%">Result No</th>
                        <th width="15%" className="money">Amount</th>
                        <th width="10%" style={{ textAlign: 'center' }}></th>
                    </tr>
                </thead>
                <tbody>
                    {
                        sort(labOrderTests, "sortOrder").map((labOrderTest, index) => {
                            return (
                                <LabOrderTestRow key={index}
                                    labOrderTest={labOrderTest}
                                    referrers={referrers}
                                    labResults={labResults}
                                    index={index}
                                    labTests={labTests}
                                    onUpdateLabTest={onUpdateLabTest}
                                    onAddLabTest={onAddLabTest}
                                    onDeleteLabTest={onDeleteLabTest}
                                    isLastLabTest={index == (labOrderTests.length - 1)}
                                />
                            )
                        })
                    }
                </tbody>
                <tfoot>
                    <tr>
                        <td colSpan={4} className="money">Sub Total</td>
                        <td className="money">{total.formatMoney()}</td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={4} className="money pt-td">Tax</td>
                        <td><input type="number" name={`Tax`} className="form-control money-input" onChange={(e) => setTax(Number(e.target.value))} value={tax} /></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={4} className="money pt-td">Discount</td>
                        <td><input type="number" name={`Discount`} className="form-control money-input" onChange={(e) => setDiscount(Number(e.target.value))} value={discount} /></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colSpan={4} className="money">Grand Total</td>
                        <td className="money">{(Number(total) + Number(tax) - Number(discount)).formatMoney()}</td>
                        <td></td>
                    </tr>
                </tfoot>
            </table>
        </div>
    );
}

const LabOrderTestRow = (props) => {
    const { labTests, referrers, labResults, index, labOrderTest, onUpdateLabTest, onAddLabTest, onDeleteLabTest, isLastLabTest } = props;

    useDidUpdateEffect(() => {
        var selectedLabTest = labTests.find(x => x.id == labOrderTest.labTestId);
        if (selectedLabTest) {
            onUpdateLabTest(index, 'unitPrice', selectedLabTest.unitPrice);
            if (labOrderTest.qty == 0) {
                onUpdateLabTest(index, 'qty', 1);
            }
            if (isLastLabTest) {
                onAddLabTest();
            }
        } else {
            onUpdateLabTest(index, 'qty', 0);
        }
    }, [labOrderTest.labTestId])

    var name = labOrderTest && labOrderTest.labTestId && labOrderTest.labTestId > 0 ? `${_serviceType}[${index}].` : '';

    return (
        <React.Fragment>
            <tr>
                <td>
                    <input name={`${name}SortOrder`} value={index + 1} hidden />
                    {index + 1}
                </td>
                <td>
                    <input name={`${name}LabTestId`} value={labOrderTest.labTestId} hidden />
                    <Select
                        value={isLastLabTest ? null : labTests.map(x => ({ value: x.id, label: `${x.name} (${x.code})` })).find(x => x.value == labOrderTest.labTestId)}
                        onChange={(e) => onUpdateLabTest(index, 'labTestId', e.value)}
                        options={labTests.map(x => ({ value: x.id, label: `${x.name} (${x.code})` }))}
                    />
                </td>
                <td>
                    <input name={`${name}ReferrerId`} value={labOrderTest.referrerId} hidden />
                    <Select
                        isClearable={true}
                        value={referrers.map(x => ({ value: x.id, label: `${x.name}` })).find(x => x.value == labOrderTest.referrerId)}
                        onChange={(e) => onUpdateLabTest(index, 'referrerId', e ? e.value : '')}
                        options={referrers.map(x => ({ value: x.id, label: `${x.name}` }))}
                    />
                </td>
                <td>
                    <input name={`${name}LabResultId`} value={labOrderTest.labResultId} hidden />
                    <input name={`${name}UnitPrice`} value={labOrderTest.unitPrice} hidden />
                    <input name={`${name}Qty`} value={labOrderTest.qty} hidden />
                    <Select
                        isClearable={true}
                        value={labResults.map(x => ({ value: x.id, label: `${x.resultNo}` })).find(x => x.value == labOrderTest.labResultId)}
                        onChange={(e) => onUpdateLabTest(index, 'labResultId', e ? e.value : '')}
                        options={labResults.map(x => ({ value: x.id, label: `${x.resultNo}` }))}
                    />
                </td>
                <td className="money pt-td">{((labOrderTest.qty || 0) * (labOrderTest.unitPrice || 0)).formatMoney()}</td>
                <td className="pt-td">
                    {
                        !isLastLabTest && <button type="button" onClick={() => onDeleteLabTest(index)} className="btn btn-xs btn-secondary">Delete</button>
                    }
                </td>
            </tr>
        </React.Fragment>
    )
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
            <OrderForm {...this.state} reset={this.clear} />
        );
    }
}

const rootElement = document.getElementById('root')
ReactDOM.render(<App ref={component => window.reactComponent = component} />, rootElement)