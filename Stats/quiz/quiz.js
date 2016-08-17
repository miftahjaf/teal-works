var studentNames = [ "Shubham", "Sakshi", "Dhvimidh", "Yug", "Dhruvi", "Arya", "Kashish", "Gitika", "Janmay", "Amoli", "Khushi", "Kushal", "Vivaan", "Chaitya", "Nirvi", "Sherman", "Shlok", "Swayam", "Viha", "Diya", "Palash", "Rishi", "Stuti", "Suhani" ];
var studentNamesAnonymous = [ "Student1", "Student2", "Student3", "Student4", "Student5", "Student6", "Student7", "Student8", "Student9", "Student10", "Student11", "Student12", "Student13", "Student14", "Student15", "Student16", "Student17", "Student18", "Student19", "Student20", "Student21", "Student22", "Student23", "Student24" ];
var studentIDs = [ "Y11sa01", "Y11sa02", "Y11sa03", "Y11sa04", "Y11sa05", "Y11sa06", "Y11sa07", "Y11sa08", "Y11sa09", "Y11sa10", "Y11sa11", "Y11sa12", "Y11sa13", "Y11sa14", "Y11sa15", "Y11sa16", "Y11sa17", "Y11sa18", "Y11sa19", "Y11sa20", "Y11sa21", "Y11sa22", "Y11sa23", "Y11sa24" ];
var currentName = "Class";
var currentTimeRange;

var attemptsPerChild = new Array();
var correctPerChild = new Array();
var scorePerChild = new Array();

var datesAttempts = new Array();
var datesCorrectAttempts = new Array();

var oldestDate;

var quizzesAttempted = new Array();
var items = new Array();
var totalAttempts = 0;
var correctAttempts = 0;
var timeSpent = 0;

var GoogleTableCSSClassNames = {
    'headerRow': 'cerebro-text',
    'tableRow': 'cerebro-text',
    'oddTableRow': 'cerebro-text'
}

var creds = new AWS.CognitoIdentityCredentials( {
  IdentityPoolId: 'ap-northeast-1:a962e236-bbff-4549-b26b-64a1fb6f10be'
} );
AWS.config.credentials = creds;
AWS.config.region = 'ap-northeast-1';
var db = new AWS.DynamoDB( { dynamoDbCrc32: false } );
//var db = new AWS.DynamoDB();
//db.listTables(function(err, data) {
//  console.log(data.TableNames);
//});

var toggleLoader = function ( show ) {
  $( '.loader' ).toggleClass( 'hidden', !show );
  $( '.content-container' ).toggleClass( 'hidden', show );
}

var toggleRemoveFilterText = function ( show ) {
  $( '.filter__remove' ).toggleClass( 'hidden', !show );
}

function getDate( offset ) {
  var today = new Date();
  var dd = today.getDate();
  var mm = today.getMonth() + 1; //January is 0!
  var yyyy = today.getFullYear();

  if ( dd < 10 ) {
    dd = '0' + dd
  }

  if ( mm < 10 ) {
    mm = '0' + mm
  }

  today = yyyy + mm + dd;
  return today;
}

function getStringDate( date ) {
  var stringdate = "";
  var dd = date.getDate();
  var mm = date.getMonth() + 1; //January is 0!
  var yyyy = date.getFullYear();

  if ( dd < 10 ) {
    dd = '0' + dd
  }

  if ( mm < 10 ) {
    mm = '0' + mm
  }

  stringdate = yyyy + mm + dd;
  return stringdate;
}

function getStudentID( studentName ) {
  var sID = -1;
  for ( var i = 0; i < studentNames.length; i++ ) {
    if ( studentNames[ i ] == studentName ) {
      currentName = studentName;
      sID = i;
      break;
    }
  }
  return sID;
}

function dbScan( lastKey ) {

  var params = {
    TableName             : 'QuizAnalytics',
    Select                : 'ALL_ATTRIBUTES',
    ReturnConsumedCapacity: 'NONE' // optional (NONE | TOTAL | INDEXES)
  };

  if ( lastKey != "" ) {
    params.ExclusiveStartKey = lastKey
  }

  db.scan( params, function ( err, data ) {
    if ( err ) console.log( err ); // an error occurred
    else {
      console.log( data ); // successful response
      for ( var i = 0; i < data.Items.length; i++ ) {
        var item = data.Items[ i ];
        if(item.StudentAndQuestionID.S.includes("test") == true) {
          continue;
        }
        fillStudentData( item );
        fillItemData( item );
        items.push( item );
      }
      // buildDataTables();

      if ( data.LastEvaluatedKey != null ) {
        console.log( "scanning more..." );
        dbScan( data.LastEvaluatedKey );
      }
      else {
      
        // SORT Analytics by Time Started
        items.sort(function(a, b){ return Number(a.TimeStarted.S) - Number(b.TimeStarted.S) });
        console.log(quizzesAttempted);
        buildDataTables();
      }
    }
  } );
}

function fillStudentData( item ) {
  var StudentAndQuestionID = item.StudentAndQuestionID.S;
  var id = StudentAndQuestionID.substring(0,7);
  if ( attemptsPerChild[ id ] == null || attemptsPerChild[ id ] == 0 ) {
    attemptsPerChild[ id ] = 1;
  }
  else {
    attemptsPerChild[ id ]++;
  }

  var correct = item.Correct.S;
  if ( correct == "true" ) {
    if ( correctPerChild[ id ] == null ) {
      correctPerChild[ id ] = 1;
    }
    else {
      correctPerChild[ id ]++;
    }

    var timeTaken = item.TimeTaken.S;
    var multiplier = 1;
    var difficulty = 0;
    var timeTaken = parseFloat (timeTaken);
    if (timeTaken < 5) {
      multiplier = 10;
    } else if (timeTaken < 10) {
      multiplier = 5;
    }else if (timeTaken < 20) {
      multiplier = 2;
    }

    var difficulty = 1;

    var difficultyBonus = difficulty * 3;
    var score = 0;
    score = (10 + difficultyBonus) * multiplier;

    if ( scorePerChild[ id ] == null ) {
      scorePerChild[ id ] = score;
    }
    else {
      scorePerChild[ id ] += score;
    }
  }
}

function fillItemData( item ) {
  if ( oldestDate == null ) {
    oldestDate = getDate();
  }

  var StudentAndQuestionID = item.StudentAndQuestionID.S;
  var dt = item.QuizDate.S;
  var id = StudentAndQuestionID.substring(0,7);

  var timeTaken = item.TimeTaken.S;
  
  timeSpent += parseFloat(timeTaken);

  if ( dt < oldestDate ) {
    oldestDate = dt;
  }

  if(quizzesAttempted[dt] == null) {
    quizzesAttempted[dt] = new Array();
    if(quizzesAttempted[dt][id] == null) {
      quizzesAttempted[dt][id] = 1;
    }
  } else {
    if(quizzesAttempted[dt][id] == null) {
      quizzesAttempted[dt][id] = 1;
    }
  }

  if ( datesAttempts[ dt ] == null ) {
    datesAttempts[ dt ] = 1;
  }
  else {
    datesAttempts[ dt ]++;
  }
  totalAttempts = totalAttempts + 1;
  var correct = item.Correct.S;
  if ( correct == "true" ) {
    correctAttempts = correctAttempts + 1;
    if ( datesCorrectAttempts[ dt ] == null ) {
      datesCorrectAttempts[ dt ] = 1;
    }
    else {
      datesCorrectAttempts[ dt ]++;
    }
  }
}

function resetData() {
  datesAttempts = new Array();
  totalAttempts = 0;
  timeSpent = 0;
  quizzesAttempted = new Array();
  attemptsPerChild = new Array();
  scorePerChild = new Array();
  correctAttempts = 0;
  datesCorrectAttempts = new Array();
  correctPerChild = new Array();
}

function getDateRange( range ) {
  if ( range == -1 ) {
    return null;
  }
  var days = range;
  var date = new Date();
  var last = new Date( date.getTime() - (days * 24 * 60 * 60 * 1000) );
  var toDate = getStringDate( date );
  var fromDate = getStringDate( last );
  var rangeArray = new Array();
  rangeArray[ "from" ] = fromDate;
  rangeArray[ "to" ] = toDate;
  return rangeArray;
}

function commafy( input ) {
  var number;

  if ( input === null ) {
    return input;
  }

  number = String( input );

  return number.replace( /(^|[^\w.])(\d{4,})/g, function ( $0, $1, $2 ) {
    return $1 + $2.replace( /\d(?=(?:\d\d\d)+(?!\d))/g, "$&," );
  } );
}

function filterTable( studentName, timeRange ) {
  if ( studentName != null ) {
    currentName = studentName;
  }
  if ( timeRange != null ) {
    currentTimeRange = timeRange;
  }
  var currentStudentID = getStudentID( currentName );

  resetData();

  for ( var i = 0; i < items.length; i++ ) {
    var item = items[ i ];
    var StudentAndQuestionID = item.StudentAndQuestionID.S;
    var dt = item.QuizDate.S;
    var id = StudentAndQuestionID.substring(0,7);

    if ( currentTimeRange != null ) {
      if ( dt < currentTimeRange[ "from" ] || dt > currentTimeRange[ "to" ] ) {
        continue;
      }
    }

    fillStudentData( item );

    if ( currentStudentID != -1 ) {
      if ( id != studentIDs[ currentStudentID ] ) {
        continue;
      }
    }

    fillItemData( item );
  }
  
  fillHTMLCommonData();
  
}

function buildDataTables() {
  
  setTimeFilters();
  timeFilterCallback( moment( oldestDate ), moment() );

  fillHTMLCommonData();

  var tableFilters = document.getElementById( 'filters_div' );
  tableFilters.style.visibility = 'visible';

  toggleLoader( false );
}

function fillHTMLCommonData() {
  var timeSpentHours = timeSpent / 3600;
  timeSpentHours = Math.round(timeSpentHours);
  var totalQuizzes = 0;
  for (var dt in quizzesAttempted) {
    for (var id in quizzesAttempted[dt]) {
      totalQuizzes++;
    }
  }

  var statsHTML = _.template( '<div class="stats-container__statcontainer col-md-4 col-xs-12 col-sm-12 col-lg-4"> <div class="stats-container__stat"> <div class="stats-container__stat__value"> <%=totalAttempts%> </div> <div class="stats-container__stat__label"> Total Attempts </div> </div> </div> <div class="stats-container__statcontainer col-md-4 col-xs-12 col-sm-12 col-lg-4"> <div class="stats-container__stat pull-left"> <div class="stats-container__stat__value"> <%=correctAttempts%> </div> <div class="stats-container__stat__label"> Correct Answers </div> </div> </div> <div class="stats-container__statcontainer col-md-4 col-xs-12 col-sm-12 col-lg-4"> <div class="stats-container__stat"> <div class="stats-container__stat__value"> <%=quizzesAttempted%> </div> <div class="stats-container__stat__label"> Quizzes Attempted </div> </div> </div>' )
  var statsData = {
    totalAttempts  : commafy( totalAttempts ),
    correctAttempts: commafy( correctAttempts ),
    quizzesAttempted : commafy( totalQuizzes )
  };
  
  $( ".stats-container__stats" ).empty().append( statsHTML( statsData ) );

  if(currentName == "Class" || currentName == "") {
    $( '.stats-container__hdr-label' ).text( "Class Quiz Statistics" );
    toggleRemoveFilterText(false);
    $( '#questiontype_div_per_class' ).toggleClass( 'hidden', false );
    $( '#questiontype_div_per_student' ).toggleClass( 'hidden', true );
  } else {
    $( '.stats-container__hdr-label' ).text( currentName + "\'s Quiz Statistics" );
    toggleRemoveFilterText(true);
    $( '#questiontype_div_per_class' ).toggleClass( 'hidden', true );
    $( '#questiontype_div_per_student' ).toggleClass( 'hidden', false );
  }
  drawStudentTable();
  drawActivityChart();
}
//dbScan("");

// Load the Visualization API and the corechart package.
google.charts.load( 'current', { 'packages': [ 'corechart', 'bar', 'table', 'controls' ] } );

// Set a callback to run when the Google Visualization API is loaded.
google.charts.setOnLoadCallback( dbScan );

function drawStudentTable() {
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Name' );
  data.addColumn( 'number', 'Attempts' );
  data.addColumn( 'number', 'Correct' );
  data.addColumn( 'number', 'Score' );
  for ( row = 0; row < studentNames.length; row++ ) {
    var name = studentNames[ row ];
    var attempts = attemptsPerChild[ studentIDs[ row ] ];
    var correct = correctPerChild[ studentIDs[ row ] ];
    var score = scorePerChild [ studentIDs[ row ] ];
    if(score == null) {
      score = 0;
    }
    if(attempts == null) {
      attempts = 0;
    }
    if(correct == null) {
      correct = 0;
    }
    data.addRow( [ name, attempts, correct, score ] );
  }

  var formatter = new google.visualization.ColorFormat();
  formatter.addRange( '0', '0.5', 'black', '#f56151' );
  formatter.format( data, 3 ); // Apply formatter to second column

  var options = {
    'title': 'Students List',
    'width': $( window ).width() * 0.9,
    'cssClassNames': GoogleTableCSSClassNames,
    'height':'100%', 'allowHtml': true
  };
  var chart = new google.visualization.Table( document.getElementById( 'studentlist_div' ) );
  chart.draw( data, options );

  google.visualization.events.addListener( chart, 'select', selectHandler );

  function selectHandler(e) {
    var selectedItem = chart.getSelection()[ 0 ];
    if ( selectedItem ) {
      var value = data.getValue( selectedItem.row, 0 );
      filterTable( value, null );
    }
  }
}

function drawActivityChart() {
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Days' );
  data.addColumn( 'number', 'Quizzes' );

  for ( var key in quizzesAttempted ) {
    if ( quizzesAttempted.hasOwnProperty( key ) ) {
      var totalQuizzes = 0;
      for( var id in quizzesAttempted[key]) {
        if ( quizzesAttempted[key].hasOwnProperty( id ) ) {
          totalQuizzes++;
        }
      }
      data.addRow( [ key, totalQuizzes ] );
    }
  }

  $( '.activity__hdr__label' ).text( 'Activity for ' + currentName );

  var options = {
    'width'    : $( window ).width() * 0.9,
    'height'   : 500,
    vAxis      : {
      'title': 'Attempts'
    },
    'animation': { "startup": true },
    fontName   : 'Source Sans Pro',
    legend     : {
      textStyle: {
        color   : '#4d4d4d',
        fontName: 'Source Sans Pro',
        fontSize: '12px',
        bold    : '400'
      }
    }
  };

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.charts.Bar( document.getElementById( 'activity_div' ) );
  chart.draw( data, google.charts.Bar.convertOptions( options ) );

}

function removeFilters() {
  filterTable( "Class", null );
}

function timeFilterCallback( start, end ) {
  $( '#reportrange span' ).html( start.format( 'MMMM D, YYYY' ) + ' - ' + end.format( 'MMMM D, YYYY' ) );
  if ( items.length == 0 ) {
    return;
  }
  var rangeArray = new Array();
  rangeArray[ "from" ] = start.format( 'YYYYMMDD' );
  rangeArray[ "to" ] = end.format( 'YYYYMMDD' );
  filterTable( null, rangeArray );
}


function exportCompleteCSV() {
  var A = [['QuizDate','StudentAndQuestionID','Answer','Correct','TimeStarted','TimeTaken']];

  for(var j=0; j<items.length; j++){
      var QuizDate = items[j].QuizDate.S;
      var StudentAndQuestionID = items[j].StudentAndQuestionID.S;
      var Answer = items[j].Answer.S;
      var Correct = items[j].Correct.S;
      var TimeStarted = items[j].TimeStarted.S;
      var TimeTaken = items[j].TimeTaken.S;

      A.push([QuizDate, StudentAndQuestionID, Answer, Correct, TimeStarted, TimeTaken]);
  }

  var csvRows = [];

  for(var i = 0; i < A.length; i++) {  
      csvRows.push(A[i].join(','));
  }
  
  var csvString = csvRows.join("\n");

  csvData = new Blob([csvString], { type: 'text/csv' }); //new way
  var csvUrl = URL.createObjectURL(csvData);

  var a         = document.createElement('a');
  // a.href        = 'data:attachment/csv,' +  encodeURIComponent(csvString);
  a.href = csvUrl;
  a.target      = '_blank';
  a.download    = 'QuizAnalytics.csv';

  console.log("Exporting Quiz CSV");
  document.body.appendChild(a);
  a.click();
}

function setTimeFilters() {
  $( '#reportrange' ).daterangepicker( {
    ranges: {
      'No Filter'    : [ moment( oldestDate ), moment() ],
      'Today'        : [ moment(), moment() ],
      'Yesterday'    : [ moment().subtract( 1, 'days' ), moment().subtract( 1, 'days' ) ],
      'Last 7 Days'  : [ moment().subtract( 6, 'days' ), moment() ],
      'Last 30 Days' : [ moment().subtract( 29, 'days' ), moment() ],
      'Last 365 Days': [ moment().subtract( 365, 'days' ), moment() ]
    }
  }, timeFilterCallback );
}

