module.exports.Vector2 = function newVec(x,y) {
    newVec.x = x;
    newVec.y = y;
    newVec.LengthSqr = function() {
        return newVec.x*newVec.x+newVec.y*newVec.y;
    }
    newVec.Length = function() {
        return sqrt(newVec.LengthSqr());
    }
}

module.exports.Add = function sum(/*Vector2Module.Vector2*/a, /*Vector2Module.Vector2*/b) {
    sum.x = a.x+b.x;
    sum.y = a.y+b.y;
}