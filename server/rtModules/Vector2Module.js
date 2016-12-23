module.exports.Create = function(x,y) {
    return new Vector2(x,y);
}

function Vector2(x,y) {
    this.x = x;
    this.y = y;
    this.LengthSqr = function() {
        return this.x*this.x+this.y*this.y;
    }
    this.Length = function() {
        return sqrt(this.LengthSqr());
    }
}

module.exports.Add = function sum(/*Vector2Module.Vector2*/a, /*Vector2Module.Vector2*/b) {
    sum.x = a.x+b.x;
    sum.y = a.y+b.y;
}