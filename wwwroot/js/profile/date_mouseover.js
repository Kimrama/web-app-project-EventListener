var datetd; 
var oldtext;

function getDaysToBegin(date) {
    const timeDiff = date - new Date().getTime();
    return Math.ceil(timeDiff / (1000 * 60 * 60 * 24));
}

function handleOver(datestr,id){
    datestr = datestr.replaceAll('/','-')
    datetd = document.getElementById(id)
    var newdate = new Date(datestr)
    datetd.style.cursor = "context-menu"
    oldtext = datetd.innerHTML
    var dayleft = getDaysToBegin(newdate)
    if(dayleft == 1){
        datetd.innerHTML = "Tomorrow"
    }
    else if(dayleft > 1){
        datetd.innerHTML = dayleft.toString() + " days left"
        datetd.style.color = "black"
    }
    else{
        datetd.innerHTML = "Started"
        datetd.style.color = "red"
    }
}

function handleOut(datestr,id){
    datetd = document.getElementById(id)
    datetd.style.cursor = "context-menu"
    datetd.innerHTML = oldtext
    datetd.style.color = "black"
}