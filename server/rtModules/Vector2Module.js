module.exports.Create = function(x,y) {
    return new Vector2(x,y);
}

function Vector2(x,y) {
    this.x = x;
    this.y = y;
    this.GetLengthSqr = function() {
        return this.x*this.x+this.y*this.y;
    }
    this.GetLength = function() {
        return Math.sqrt(this.GetLengthSqr());
    }
}

module.exports.Add = function(/*Vector2Module.Vector2*/a, /*Vector2Module.Vector2*/b) {
    return new module.exports.Create(a.x+b.x, a.y+b.y);
}

module.exports.Subtract = function(/*Vector2Module.Vector2*/a, /*Vector2Module.Vector2*/b) {
    return new module.exports.Create(a.x-b.x, a.y-b.y);
}

module.exports.Multiply = function(/*Vector2Module.Vector2*/a, /*float*/mul) {
    return new module.exports.Create(a.x*mul, a.y*mul);
}

module.exports.Equals = function(/*Vector2Module.Vector2*/a, /*Vector2Module.Vector2*/b) {
    return (a.x==b.x) && (a.y==b.y);
}