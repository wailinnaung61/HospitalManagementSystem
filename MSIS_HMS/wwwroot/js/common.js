$(function () {
    $('.btn-click').on('click', function (e) {
        location.href = $(this).data('href');
    });
    $.fn.autoFade = function () {
        return this.each(function () {
            $(this).fadeTo(5000, 500).slideUp(500, function () {
                $(this).slideUp(500);
            });
        });
    };
})

const isEmpty = str => str == undefined || str == null || (str + '') == '';

const calculateQtyInSmallestUnit = (packingUnits, unitId, qty) => {
    var qtyInSmallestUnit = 1;
    var currentUnit = packingUnits.find(x => x.unitId == unitId);
    if (currentUnit) {
        for (var i = Math.max.apply(Math, packingUnits.map(x => x.unitLevel)); i > currentUnit.unitLevel; i--) {
            qtyInSmallestUnit *= packingUnits[i - 1].qtyInParent;
        }
    }
    qty *= qtyInSmallestUnit;
    return qty;
}

const calculateTotal = (arr, key) => {
    var total = 0;
    arr.map(x => total += Number(x[key]));
    return total;
}

const calculateBulkTotal = (arr, key, tax, discount) => {
    var bulktotal = 0;
    var total=0
    arr.map(x => total += x[key]);
    var bulktotal=(total-(total*(discount/100))) +tax;         
    
    return parseFloat(bulktotal);
}

function SaveAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' Save Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}

function capitalize(object) {
    var isArray = Array.isArray(object);
    for (let key in object) {
        let value = object[key];
        let newKey = key;
        if (!isArray) { // if it is an object 
            delete object[key]; // firstly remove the key 
            newKey = key.charAt(0).toUpperCase() + key.slice(1); // secondly generate new key (capitalized) 
        }
        let newValue = value;
        if (typeof value != "object") { // if it is not an object (array or object in fact), stops here 
            if (typeof value == "string") {
                newValue = value.toUpperCase(); // if it is a string, capitalize it 
            }
        } else { // if it is an object, recursively capitalize it 
            newValue = capitalize(value);
        }
        object[newKey] = newValue;
    }
    return object;
} 

function EditAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' update Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}

function DeleteAlert(position, icon, title) {
    Swal.fire({
        position: position,
        icon: icon,
        showCloseButton: true,
        title: title + ' delete Successfully',
        showConfirmButton: false,
        ConfirmButtonText: "Close",
        timer: 2000
    })
}

function DeleteConfirm(url, id) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = url + id;
        }
    })
}

function DeleteFnc(callback) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            callback();
        }
    })
}
