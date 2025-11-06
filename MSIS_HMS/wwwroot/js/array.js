function sort(array, field) {
    return array.sort(function (a, b) { return (a[field] > b[field]) ? 1 : ((b[field] > a[field]) ? -1 : 0); })
}

function arrayRemove(arr, value) {
    return arr.filter(function (ele) { return ele != value; });
}

function arrayRemoveByKey(arr, key, value) {
    return arr.filter(function (ele) { return ele[key] != value; });
}

function arrayRemoveByIndex(arr, index) {
    if (index > -1) {
        return arr.splice(index, 1);
    }
    return arr;
}

function indexOfByKey(obj_list, key, value) {
    for (var index = 0; index < obj_list.length; index++) {
        if (obj_list[index][key] === value) return index;
    }
    return -1;
}

function sum(arr, key) {
    return arr.length > 0 ? (arr.reduce((a, b) => {
        return { [key]: a[key] + b[key] }
    }))[key] : 0
}

function groupBy(xs, key) {
    return xs.reduce(function (rv, x) {
        (rv[x[key]] = rv[x[key]] || []).push(x);
        return rv;
    }, {});
};

function objToArray(obj) {
    return Object.keys(obj).map(function (objkey) {
        return { "key": objkey, "list": obj[objkey] };
    });
}
function clone(arr) {
    var newArr = [];
    arr.map(item => newArr.push(item));
    return newArr;
};