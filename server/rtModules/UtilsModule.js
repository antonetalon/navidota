module.exports.Clone = function(obj) {
    if (obj === null || typeof obj !== 'object')
        return obj; 
    var temp = obj.constructor(); // give temp the original obj's constructor
    for (var key in obj)
        temp[key] = clone(obj[key]); 
    return temp;
}