
var colors = [
    ["#461220", "#8c2f39", "#b23a48", "#fcb9b2", "#fed0bb"],
    ["#e63946", "#f1faee", "#a8dadc", "#457b9d", "#1d3557"],
    ["#fff275", "#ff8c42", "#ff3c38", "#a23e48", "#6c8ead"],
    ["#ffdda1", "#ffd151", "#f8c537", "#edb230", "#e77728"],
    ["#818479", "#b5cbb7", "#d2e4c4", "#e4e9b2", "#e7e08b"],
    ["#e0fbfc", "#c2dfe3", "#9db4c0", "#5c6b73", "#253237"],
    ["#114b5f", "#1a936f", "#88d498", "#c6dabf", "#f3e9d2"],
    ["#3c4156", "#14213d", "#fca311", "#e5e5e5", "#ffffff"],
    ["#012622", "#003b36", "#ece5f0", "#e98a15", "#59114d"],
    ["#577590", "#f3ca40", "#f2a541", "#f08a4b", "#d78a76"],
    ["#bce784", "#5dd39e", "#348aa7", "#525174", "#513b56"],
    ["#0e1116", "#374a67", "#616283", "#9e7b9b", "#cb9cf2"],
    ["#c2c1c2", "#42213d", "#683257", "#bd4089", "#f51aa4"],
    ["#031a6b", "#033860", "#087ca7", "#004385", "#05b2dc"],
    ["#306b34", "#1c5420", "#c6ffca", "#77ea7f", "#17b522"],
    ["#23c9ff", "#7cc6fe", "#ccd5ff", "#e7bbe3", "#c884a6"],
    ["#3c1518", "#69140e", "#a44200", "#d58936", "#fff94f"],
    ["#d00000", "#ffba08", "#3f88c5", "#032b43", "#136f63"],
    ["#ff4e00", "#8ea604", "#f5bb00", "#ec9f05", "#bf3100"],
    ["#2d3142", "#4f5d75", "#bfc0c0", "#ffffff", "#ef8354"],
    ["#ef798a", "#f7a9a8", "#7d82b8", "#613f75", "#e5c3d1"],
    ["#20bf55", "#0b4f6c", "#01baef", "#fbfbff", "#757575"],
    ["#393e41", "#d3d0cb", "#e2c044", "#587b7f", "#1e2019"],
    ["#0a369d", "#4472ca", "#5e7ce2", "#92b4f4", "#cfdee7"],
    ["#ffdda1", "#ffbf51", "#f7b036", "#eda72f", "#e8a127"],
    ["#813405", "#d35c13", "#f9863e", "#f7c3a3", "#f9d7c2"],
    ["#843636", "#331313", "#990303", "#cc3b3b", "#fc4e4e"],
    ["#190b28", "#685762", "#9b9987", "#efa9ae", "#e55381"],
];
// an array of colors our boids will inherit their strokecolors from. this is put at the top because otherwise it's in my way
function Boid(point, rotation, colorPreset) {
    // boid object creator
    this.path = new Path({
        strokeColor: randChoice(colorPreset),
        strokeWidth: "9",
        pivot: [-7.5, 0],
        applyMatrix: false,
        rotation: rotation
    });
    // create a new pat, and give it desired attributes
    this.path.add([-15, 10], [0, 0], [-15, -10]);
    // make the angle shape
    this.path.position = point;
    // move to the correct location
    this.counter = 0;
    // maybe this will be used later
    this.jitter = 0;
    // the amount the boid should jitter back and forth
}

Boid.prototype = {
    moveBoid: function() {
        // a basic function dictating the boid's movement
        var vector = new Point({
            angle: this.path.rotation,
            length: boidSpeed
        });
        // create a vector pointing the direction that the triangle is facing and give it a length of how fast we want the boids to move (+30 so that it goes towards a point of the triangle)
        this.path.position += vector;
        // move it
        keepInView(this.path);
            // loop the boid around the screen if it went off
    },
    checkNearby: function() {
        var nearby = [];
        // array for nearby boids
        var nearest;
        // variable declaration for nearest
        var distTo = viewDist;
        // set the current lowest distance to the view distance
        boids.forEach(function(boid) {
            // for every boid
            var curDist = distFormula(this.path.position, boid.path.position);
            // get the distance
            if (curDist <= viewDist) {
                // if the distance is within th
                var angleTo = (boid.path.position - this.path.position).angle;
                // get the angle from this boid to the one we are iterating over
                if (!(angleTo <= -135 || angleTo >= 135)) {
                    // limit the boid's view by filtering out others that are in a 90 degree cone behind it
                    if (curDist !== 0) {
                        // if the distance to is not 0 (so we don't select ourselves)
                        nearby.push(boid);
                        // push the boid into our array
                        if (curDist < distTo) {
                            // if this current distance to is lower than the current lowest
                            nearest = boid;
                            // reassign nearest
                            distTo = curDist;
                            // reassign the distance
                        }
                    }
                }
            }
        }, this);
        if (nearest !== undefined) {
            // if a nearest boid exists
            var step = 0;
            // set the step number to be 0
            var nearestVector = new Point({
                angle: nearest.path.rotation,
                length: nearest.speed,
                applyMatrix: false
            });
            var curVector = new Point({
                angle: this.path.rotation,
                length: this.speed,
                applyMatrix: false
            });
            // create the corresponding vectors for the nearest and this boid
            while (distFormula(this.path.position + curVector * step, nearest.path.position + nearestVector * step) <= distFormula(this.path.position, nearest.path.position)) {
                // while the distance between the boids on each iteration does not increase (they are heading towards each other)
                step++;
                // increment the step
                if (distFormula(this.path.position + curVector * step, nearest.path.position + nearestVector * step) <= personalSpace) {
                    // detecting if they will collide sometime in the future
                    this.avoidNearby(nearest);
                    // if so then avoid it
                }
                if (step > maxSteps) {
                    break;
                    // if we have done the number of steps we want and there have been no detected collisions, then break
                }
            }
        }
        if (nearby.length) {
            // if there are other boids nearby
            this.assimilateNearby(nearby);
            this.swerveToCenter(nearby);
            // make an attempt to assimiliate to the nearby boid's angles and swerve to the center of the local group
        }
    },
    avoidNearby: function(nearest) {
        var moved = this.path.position + new Point({angle: this.path.rotation, length: viewDist * 2});
        // a point representing where the path would be 
        var checker = (nearest.path.position.x - this.path.position.x) * (moved.y - this.path.position.y) - (nearest.path.position.y - this.path.position.y) * (moved.x - this.path.position.x);
        if (checker > 0) {
            this.path.rotate(rotationStrength);
        } else {
            this.path.rotate(-rotationStrength);
        }
        // ripped off stackoverflow, this is a method for determining if the nearest boid's position is to the left of right of the current boid. if to the right then turn left and vice versa
    },
    assimilateNearby: function(nearby) {
        var nearbyAngles = nearby.map(function(nearby) {
            return nearby.path.rotation;
        }, this);
        // array of the rotations of nearby boids
        if (nearbyAngles.length) {
            this.path.rotate((nearbyAngles.reduce(function(a, b) { return a+b; }) / nearbyAngles.length - this.path.rotation) * assimStrength);
            // get the average of the rotations of all nearby boids and rotate towards it
        }
    },
    swerveToCenter: function(nearby) {
        var nearbyPositions = nearby.map(function(nearby) {
            return [nearby.path.position.x, nearby.path.position.y];
            // return an array of [x,y] for each boid
        }, this);
        var xVals = 0;
        var yVals = 0;
        nearbyPositions.forEach(function(vals) {
            xVals += Math.abs(vals[0]);
            yVals += Math.abs(vals[1]);
        });
        var vector = new Point(xVals / nearby.length, yVals / nearby.length) - this.path.position;
        // get the total average of the x and y coords, then create a vector
        this.path.rotate((vector.angle - this.path.rotation) * centerStrength);
        // rotate towards the generated vector
    },
    jitterRotation: function() {
        this.path.rotate(this.jitter);
        // rotate the boid by the jitter amount
    },
    newJitter: function() {
        this.jitter = randFloat(-jitterStrength, jitterStrength);
        // assign a new jitter maount
    }
};
var boids = [];
// an array to store our boids in
var rotationStrength = 1.5;
var jitterStrength = 3;
var assimStrength = 1/10;
var centerStrength = 1/20;
// variables for how strong the avoidance, assimilation, and cohesion forces should be
var boidSpeed = 9;
// how fast the boids should move
var personalSpace = 60;
// at which point the boids should try to avoid each other
var numBoids = 45;
var spawnedBoids = 0;
// number of boids that we want to spawn in
var maxSteps = 8;
// how many frames each boid should simulate to detect future collisions
var viewDist = 85;
// the radius of which a boid will detect nearby boids
var colorSet = randChoice(colors);
// set the initial colorset for the boids
var hueShift = false;
// start off without the hueshift effect
var counter = -30;
// counter for the breathing effect
setSpawnInterval();
// spawn the boids

function onFrame(frame) {
    // every frame (30/s or 60/s)
    boids.forEach(function(boid) {
        // for every boid
        boid.jitterRotation();
        boid.checkNearby();
        boid.moveBoid();
        // perform necessary actions
    });
    if (frame.count % 4 === 0) {
        // every 4 frames
        if (hueShift) {
            // if there is a hueshift effect active then shift the hue of each boid
            boids.forEach(function(boid) {
                boid.path.strokeColor.hue += 5;
            });
        }
        if (counter < 0) {
        boids.forEach(function(boid) {
            boid.path.strokeColor.brightness -= 0.005;
            boid.path.strokeColor.saturation -= 0.005;
        });
        } else if (counter > 0) {
            boids.forEach(function(boid) {
                boid.path.strokeColor.brightness += 0.005;
                boid.path.strokeColor.saturation += 0.005;
            });
        }
        counter += 1;
        if (counter > 50) {
            counter = -50;
        }
        // various stuff for a 'breathing' effect, where the saturation and brightness of each boid fluctuates up and down
    }
    if (frame.count % 60 === 0) {
        boids.forEach(function(boid) {
            boid.newJitter();
        });
        // every 60 frames give each boid a new random jitter direction
    }
}

function setSpawnInterval() {
    // for spawning the boids
    var spawnInterval = setInterval(function() {
        // create a spawn interval
        boids.push(new Boid(view.bounds, randFloat(-1, 1) * 180, colorSet));
        // spawn a new boid at the edge of the screen with random rotation and color
        spawnedBoids++;
        // increment the number of boids we have spawned
        if (spawnedBoids >= numBoids) {
            // if we have spawned as many as we wanted
            clearInterval(spawnInterval);
            // clear the interval
        }
    }, 150);
    // repeat the spawn interval every 30s
}

function onKeyDown(event) {
    if (event.key === "space") {
        project.clear();
        boids.forEach(function(boid) {
            boid.path.remove();
        });
        boids = [];
        spawnedBoids = 0;
        setSpawnInterval();
        // if the key pressed is the spacebar, then recreate the boids
    } 
    else if (event.key === "control") {
        if (randint(1, 5) === 1) {
            hueShift = true;
            boids.forEach(function(boid) {
                boid.path.strokeColor = randChoice(randChoice(colors));
            });
        } else {
            hueShift = false;
            colorSet = randChoice(colors);
            boids.forEach(function(boid) {
                boid.path.strokeColor = randChoice(colorSet);
            });
        }
        // if the key pressed is ctrl, recolor the boids and have a chance to give them a hueshift effect
    } 
    else if (event.key === "left") {
        boids.forEach(function(boid) {
            boid.path.rotate(-10);
        });
        // if key pressed is left arrow then rotate counterclockwise
    } 
    else if (event.key === "right") {
        boids.forEach(function(boid) {
            boid.path.rotate(10);
        });
        // if key pressed is right arrow then rotate counterclockwise
    } 
    else if (event.key === "up") {
        boidSpeed++;
        // if key pressed is up arrow then increment boid speed
    } 
    else if (event.key === "down") {
        if (boidSpeed >= 1) {
            boidSpeed--;
        }
        // if key pressed is down arrow then decrement boid speed, stopping it at 0 (negative vector lengths still result in positvie)
    } 
    else if (event.key === "shift") {
        rotationStrength = randFloat(1, 5);
        jitterStrength = randFloat(0, 10);
        assimStrength = randFloat(0, 0.5);
        centerStrength = randFloat(0, 0.5);
        // if key pressed is shift then reassign the key variables dicating movement, just for fun
    }
    else if (event.key === "[") {
        if (boids.length) {
            boids.pop().path.remove();
        }
        // if key pressed is lbracket then remove a boid
    } 
    else if (event.key === "]") {
        boids.push(new Boid([0, 0], randFloat(-1, 1) * 180, colorSet));
        // if key pressed is rbracket then add a boid
    }
}

function keepInView(item) {
    // a function that is shown on the paperjs website, used to keep items in view by looping them around the screen
    var position = item.position;
    var viewBounds = view.bounds;
    if (position.isInside(viewBounds))
        return;
    var itemBounds = item.bounds;
    if (position.x > viewBounds.width + 5) {
        position.x = -item.bounds.width;
    }

    if (position.x < -itemBounds.width - 5) {
        position.x = viewBounds.width;
    }

    if (position.y > viewBounds.height + 5) {
        position.y = -itemBounds.height;
    }

    if (position.y < -itemBounds.height - 5) {
        position.y = viewBounds.height;
    }
}

function randint(min, max) {
    // random.randint
    return Math.floor(Math.random() * (max - min + 1) + min);
}
function randFloat(min, max) {
    // random.uniform
    return (Math.random() * (max - min) + min);
}
function randChoice(arr) {
    // random.choice
    return arr[Math.floor(Math.random()*arr.length)];
}
function distFormula(one, two) {
    // formula for getting the distance between two objects
    return Math.sqrt(Math.pow(one.x - two.x, 2) + Math.pow(one.y - two.y, 2));
}
    