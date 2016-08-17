var studentNames = [ "Shubham", "Sakshi", "Dhvimidh", "Yug", "Dhruvi", "Arya", "Kashish", "Gitika", "Janmay", "Amoli", "Khushi", "Kushal", "Vivaan", "Chaitya", "Nirvi", "Sherman", "Shlok", "Swayam", "Viha", "Diya", "Palash", "Rishi", "Stuti", "Suhani" ];
var studentNamesAnonymous = [ "Student1", "Student2", "Student3", "Student4", "Student5", "Student6", "Student7", "Student8", "Student9", "Student10", "Student11", "Student12", "Student13", "Student14", "Student15", "Student16", "Student17", "Student18", "Student19", "Student20", "Student21", "Student22", "Student23", "Student24" ];
var studentIDs = [ "Y11sa01", "Y11sa02", "Y11sa03", "Y11sa04", "Y11sa05", "Y11sa06", "Y11sa07", "Y11sa08", "Y11sa09", "Y11sa10", "Y11sa11", "Y11sa12", "Y11sa13", "Y11sa14", "Y11sa15", "Y11sa16", "Y11sa17", "Y11sa18", "Y11sa19", "Y11sa20", "Y11sa21", "Y11sa22", "Y11sa23", "Y11sa24" ];
var currentName = "Class";
var currentTimeRange;
var subtopicIDs = [ "MAlg", "MSI", "MMEN", "MRAP", "MSET", "MNS"];
var questionTypeIDs = new Array();
var questionTypeNames = new Array();
var questionTypeAttempted = new Array();
var questionTypeCorrect = new Array();
var questionTypeAttemptedPerChild = new Array();
var questionTypeCorrectPerChild = new Array();
var subtopicNames = [ "Simplifying Algebraic Expressions", "Simple Interest", "Mensuration", "Ratio and Proportions", "Sets", "Number System" ];
var subtopicAttempts = new Array();
var subtopicCorrectAttempts = new Array();

var tiers = [90,70];

var attemptsPerChild = new Array();
var correctPerChild = new Array();

for ( var i = 0; i < subtopicIDs.length; i++ ) {
  subtopicAttempts[ subtopicIDs[ i ] ] = 0;
  subtopicCorrectAttempts[ subtopicIDs[ i ] ] = 0;
}

var datesAttempts = new Array();
var datesCorrectAttempts = new Array();

var oldestDate;

var items = new Array();
var totalAttempts = 0;
var correctAttempts = 0;
var videosWatched = 0;
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

$( document ).ready( function () {
  $.ajax( {
    type    : "GET",
    url     : "Mapping.csv",
    dataType: "text",
    success : function ( data ) {
      processData( data );
    }
  } );
} );

function processData( allText ) {
  var allTextLines = allText.split( /\r\n|\n/ );
  for ( var i = 0; i < allTextLines.length; i++ ) {
    var data = allTextLines[ i ].split( ',' );
    var tarr = [];
    for ( var j = 2; j < data.length; j++ ) {
      tarr.push( data[ j ] );
    }
    var code = data[ 0 ] + data[ 1 ];
    questionTypeIDs.push( code );
    questionTypeAttemptedPerChild[ code ] = new Array();
    questionTypeCorrectPerChild[ code ] = new Array();
    for(var j = 0 ; j < studentIDs.length ; j++) {
      questionTypeAttemptedPerChild[ code ][ studentIDs[j] ] = 0;
      questionTypeCorrectPerChild[ code ][ studentIDs[j] ] = 0;
    }
    questionTypeNames[ code ] = tarr;
    questionTypeAttempted[ code ] = 0;
    questionTypeCorrect[ code ] = 0;
  }
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
  var myParam = location.search.split( 'student=' )[ 1 ];

  var params = {
    TableName             : 'Analytics',
    Select                : 'ALL_ATTRIBUTES',
    ReturnConsumedCapacity: 'NONE' // optional (NONE | TOTAL | INDEXES)
  };

  if ( myParam != null ) {
    var sID = getStudentID( myParam );

    if ( sID != -1 )

      params.ScanFilter = { // optional (map of attribute name to Condition)

        StudentID: {
          ComparisonOperator: 'EQ', // (EQ | NE | IN | LE | LT | GE | GT | BETWEEN |
                                    //  NOT_NULL | NULL | CONTAINS | NOT_CONTAINS | BEGINS_WITH)
          AttributeValueList: [ { S: studentIDs[ sID ] } ]
        }

      };
  }

  if ( lastKey != "" ) {
    params.ExclusiveStartKey = lastKey
  }

  db.scan( params, function ( err, data ) {
    if ( err ) console.log( err ); // an error occurred
    else {
      console.log( data ); // successful response
      for ( var i = 0; i < data.Items.length; i++ ) {
        var item = data.Items[ i ];
        if(item.StudentID.S == "test") {
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
        
        buildDataTables();
      }
    }
  } );
}

function fillStudentData( item ) {
  var id = item.StudentID.S;
  var assId = item.AssessmentItemID.S;
  if ( assId.indexOf( "VIDEO" ) != 0 ) {
    if ( attemptsPerChild[ id ] == null ) {
      attemptsPerChild[ id ] = 1;
    }
    else {
      attemptsPerChild[ id ]++;
    }

    var correct = item.Correct.BOOL;
    if ( correct == true ) {
      if ( correctPerChild[ id ] == null ) {
        correctPerChild[ id ] = 1;
      }
      else {
        correctPerChild[ id ]++;
      }
    }
  }

}

function fillItemData( item ) {
  if ( oldestDate == null ) {
    oldestDate = getDate();
  }

  var assId = item.AssessmentItemID.S;
  var dt = item.Date.S;
  var id = item.StudentID.S;
  var difficulty = item.Difficulty.N;

  var timeTaken = item.TimeTaken.N;
  
  timeSpent += parseFloat(timeTaken);

  if ( dt < oldestDate ) {
    oldestDate = dt;
  }

  if ( datesAttempts[ dt ] == null ) {
    datesAttempts[ dt ] = 1;
  }
  else {
    datesAttempts[ dt ]++;
  }
  if ( assId.indexOf( "VIDEO" ) == 0 ) {
    videosWatched = videosWatched + 1;
  }
  else {
    totalAttempts = totalAttempts + 1;
    var correct = item.Correct.BOOL;
    if ( correct == true ) {
      correctAttempts = correctAttempts + 1;
      if ( datesCorrectAttempts[ dt ] == null ) {
        datesCorrectAttempts[ dt ] = 1;
      }
      else {
        datesCorrectAttempts[ dt ]++;
      }
    }

    var foundSubtopic = false;
    for ( var j = 0; j < subtopicIDs.length; j++ ) {
      var subtopicID = subtopicIDs[ j ];
      if ( assId.indexOf( subtopicID ) == 0 ) {
        var typecode = "";
        if ( assId.search( "t" ) != -1 ) {
          var questionCode = (difficulty + assId.substr( assId.search( "t" ), 2 ));
          typecode = (subtopicID + questionCode);
          if ( questionTypeAttempted[ typecode ] != null ) {
            questionTypeAttempted[ typecode ] += 1;
            questionTypeAttemptedPerChild[ typecode ][ id ] += 1;
          }
        }
        subtopicAttempts[ subtopicID ]++;
        if ( correct == true ) {
          subtopicCorrectAttempts[ subtopicID ]++;
          if ( questionTypeCorrect[ typecode ] != null ) {
            questionTypeCorrect[ typecode ] += 1;
            questionTypeCorrectPerChild[ typecode ][ id ] += 1;
          }
        }
        foundSubtopic = true;
        break;
      }
    }
    if ( foundSubtopic == false ) {
      console.log( "Unhandled assessment type " + assId + "," + id );
    }
  }
}

function getSubtopicName( subtopicID ) {
  for ( var j = 0; j < subtopicIDs.length; j++ ) {
    if ( subtopicID.indexOf( subtopicIDs[ j ] ) == 0 ) {
      return subtopicNames[ j ];
    }
  }
  return "";
}

function resetData() {
  datesAttempts = new Array();
  videosWatched = 0;
  totalAttempts = 0;
  timeSpent = 0;
  attemptsPerChild = new Array();
  correctAttempts = 0;
  datesCorrectAttempts = new Array();
  correctPerChild = new Array();
  subtopicAttempts = new Array();
  subtopicCorrectAttempts = new Array();

  for ( var key in questionTypeAttempted ) {
    questionTypeAttempted[ key ] = 0;
    for(var i = 0 ; i < studentIDs.length ; i++) {
      questionTypeAttemptedPerChild[key][studentIDs[i]] = 0;
    }
  }
  for ( var key in questionTypeCorrect ) {
    questionTypeCorrect[ key ] = 0;
    for(var i = 0 ; i < studentIDs.length ; i++) {
      questionTypeCorrectPerChild[key][studentIDs[i]] = 0;
    }
  }

  for ( var i = 0; i < subtopicIDs.length; i++ ) {
    subtopicAttempts[ subtopicIDs[ i ] ] = 0;
    subtopicCorrectAttempts[ subtopicIDs[ i ] ] = 0;
  }
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
    var assId = item.AssessmentItemID.S;
    var dt = item.Date.S;
    var id = item.StudentID.S;

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
  timeFilterCallback( moment( oldestDate ), moment() );
  setTimeFilters();
  
  fillHTMLCommonData();

  drawQuestionTypeTablePerClass();

  var tableFilters = document.getElementById( 'filters_div' );
  tableFilters.style.visibility = 'visible';

  toggleLoader( false );
}

function fillHTMLCommonData() {
  var timeSpentHours = timeSpent / 3600;
  timeSpentHours = Math.round(timeSpentHours);

  var statsHTML = _.template( '<div class="stats-container__statcontainer col-md-3 col-xs-12 col-sm-12 col-lg-3"> <div class="stats-container__stat"> <div class="stats-container__stat__value"> <%=totalAttempts%> </div> <div class="stats-container__stat__label"> Total Attempts </div> </div> </div> <div class="stats-container__statcontainer col-md-3 col-xs-12 col-sm-12 col-lg-3"> <div class="stats-container__stat pull-left"> <div class="stats-container__stat__value"> <%=correctAttempts%> </div> <div class="stats-container__stat__label"> Correct Answers </div> </div> </div> <div class="stats-container__statcontainer col-md-3 col-xs-12 col-sm-12 col-lg-3"> <div class="stats-container__stat pull-left"> <div class="stats-container__stat__value"> <%=videosWatched%> </div> <div class="stats-container__stat__label"> Videos Watched </div> </div> </div> <div class="stats-container__statcontainer col-md-3 col-xs-12 col-sm-12 col-lg-3"> <div class="stats-container__stat"> <div class="stats-container__stat__value"> <%=timeSpent%> </div> <div class="stats-container__stat__label"> Hours Spent </div> </div> </div>' )
  var statsData = {
    totalAttempts  : commafy( totalAttempts ),
    correctAttempts: commafy( correctAttempts ),
    videosWatched  : commafy( videosWatched ),
    timeSpent : commafy( timeSpentHours )
  };
  
  $( ".stats-container__stats" ).empty().append( statsHTML( statsData ) );

  if(currentName == "Class" || currentName == "") {
    $( '.stats-container__hdr-label' ).text( "Class Statistics" );
    toggleRemoveFilterText(false);
    $( '#questiontype_div_per_class' ).toggleClass( 'hidden', false );
    $( '#questiontype_div_per_student' ).toggleClass( 'hidden', true );
    drawQuestionTypeTablePerClass();
  } else {
    $( '.stats-container__hdr-label' ).text( currentName + "\'s Statistics" );
    toggleRemoveFilterText(true);
    drawQuestionTypeTablePerStudent();
    $( '#questiontype_div_per_class' ).toggleClass( 'hidden', true );
    $( '#questiontype_div_per_student' ).toggleClass( 'hidden', false );
  }

  drawClassAccuracyChart();
  drawSubtopicAttemptsChart();
  drawSubtopicAccuracyChart();
  drawActivityChart();
  drawAccuracyChart();
  drawStudentTable();


}
//dbScan("");

// Load the Visualization API and the corechart package.
google.charts.load( 'current', { 'packages': [ 'corechart', 'bar', 'table', 'controls' ] } );

// Set a callback to run when the Google Visualization API is loaded.
google.charts.setOnLoadCallback( dbScan );

function drawClassAccuracyChart() {

  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Answer' );
  data.addColumn( 'number', 'Attempts' );
  var inCorrectAttempts = totalAttempts - correctAttempts;
  data.addRows( [
    [ 'Correct', correctAttempts ],
    [ 'Incorrect', inCorrectAttempts ]
  ] );

  $( '.class-accuracy__hdr__label' ).text( currentName + "\'s Accuracy" );

  var options = {
    'width'          : $( window ).width() * 0.9,
    'height'         : 500,
    'colors'         : [ '#55cc88', '#f56151' ],
    'animation'      : { 'startup': true },
    is3D             : true,
    backgroundColor  : '#e5e5e5',
    fontName         : 'Source Sans Pro',
    legend           : {
      textStyle: {
        color   : '#4d4d4d',
        fontName: 'Source Sans Pro',
        fontSize: '12px',
        bold    : '400'
      }
    },
    pieSliceTextStyle: {
      color   : '#333',
      fontName: 'Source Sans Pro',
      fontSize: '12px',
      bold    : '400'
    }
  };

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.PieChart( document.getElementById( 'classAccuracy_div' ) );
  chart.draw( data, options );

}

function drawSubtopicAttemptsChart() {
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Subtopic' );
  data.addColumn( 'number', 'Attempts' );
  for ( var i = 0; i < subtopicIDs.length; i++ ) {
    data.addRow( [ subtopicNames[ i ], subtopicAttempts[ subtopicIDs[ i ] ] ] );
  }

  $( '.subtopic-attempts__hdr__label' ).text( 'Subtopics Attempted by ' + currentName );

  var options = {
    'width'          : $( window ).width() * 0.9,
    'height'         : 500,
    'animation'      : { "startup": true },
    //'colors'         : [ '#55cc88', '#4285f4', '#fd9c78', '#ffe84e', '#f56151', ],
    is3D             : true,
    backgroundColor  : '#e5e5e5',
    fontName         : 'Source Sans Pro',
    legend           : {
      textStyle: {
        color   : '#4d4d4d',
        fontName: 'Source Sans Pro',
        fontSize: '12px',
        bold    : '400'
      }
    },
    pieSliceTextStyle: {
      color   : '#333',
      fontName: 'Source Sans Pro',
      fontSize: '12px',
      bold    : '400'
    }
  };

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.PieChart( document.getElementById( 'subtopicAttempts_div' ) );
  chart.draw( data, options );

}

function drawSubtopicAccuracyChart() {
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Sub Topics' );
  data.addColumn( 'number', 'Correct' );
  data.addColumn( 'number', 'Incorrect' );
  for ( var i = 0; i < subtopicIDs.length; i++ ) {
    data.addRow( [ subtopicNames[ i ], subtopicCorrectAttempts[ subtopicIDs[ i ] ], subtopicAttempts[ subtopicIDs[ i ] ] - subtopicCorrectAttempts[ subtopicIDs[ i ] ] ] );
  }

  var options = {
    'width'    : $( window ).width() * 0.8,
    'height'   : 500,
    'isStacked': 'true',
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
  var chart = new google.charts.Bar( document.getElementById( 'subtopicAccuracy_div' ) );
  chart.draw( data, google.charts.Bar.convertOptions( options ) );

}

function drawActivityChart() {
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Days' );
  data.addColumn( 'number', 'Correct' );
  data.addColumn( 'number', 'Incorrect' );
  for ( var key in datesAttempts ) {
    if ( datesAttempts.hasOwnProperty( key ) ) {
      data.addRow( [ key, datesCorrectAttempts[ key ], datesAttempts[ key ] - datesCorrectAttempts[ key ] ] );
    }
  }

  $( '.activity__hdr__label' ).text( 'Activity for ' + currentName );

  var options = {
    'width'    : $( window ).width() * 0.9,
    'height'   : 500,
    'isStacked': 'true',
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



function drawAccuracyChart() {
  var currentStudentID = getStudentID( currentName );

  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Sums');
  data.addColumn( 'number', 'Difficulty' );
  data.addColumn({type: 'string', role: 'style'});
  var sumNumber = 0;
  for ( var i = 0; i < items.length; i++ ) {
    var item = items[ i ];
    var dt = item.Date.S;
    var id = item.StudentID.S;

    if ( currentTimeRange != null ) {
      if ( dt < currentTimeRange[ "from" ] || dt > currentTimeRange[ "to" ] ) {
        continue;
      }
    }

    if ( currentStudentID != -1 ) {      
      if ( id != studentIDs[ currentStudentID ] ) {
        continue;
      }
    }
    else
    {
      break;
    }

      var AssessmentItemID = item.AssessmentItemID.S;
      var Correct = item.Correct.BOOL.toString();
      var Difficulty = Number(item.Difficulty.N.toString());
      if(Difficulty <= 0)
      {
        console.log(Difficulty);
      }
      if(Correct == "true")
      {
          data.addRow( [ sumNumber.toString(), Difficulty, 'color: #55cc88' ] );
      }
      else
      {
        data.addRow( [ sumNumber.toString(), Difficulty, 'color: #f56151' ] );
      }
      sumNumber = sumNumber + 1;
      
  }
  
  /*for ( var key in datesAttempts ) {
    if ( datesAttempts.hasOwnProperty( key ) ) {
      var perc = datesCorrectAttempts[ key ] * 100 / datesAttempts[ key ];
      data.addRow( [ key, perc, 'color: #FFFF00' ] );
    }
  }*/

  $( '.accuracy__hdr__label' ).text( 'Accuracy for ' + currentName );

  var w = Math.max(data.getNumberOfRows() * 2.2, $( window ).width() * 0.9);
  var options = {
    //'width'    : $( window ).width() * 0.9,
    'height'   : 300,

    hAxis: {title: 'Sums', titleTextStyle: {color: 'red'}},
    width: w,
    
    allowHtml: true,
    legend:'none',
    vAxis      : {
      'title': 'Difficulty'
    },
    fontName   : 'Source Sans Pro'    
  };

  // Instantiate and draw our chart, passing in some options.
  var chart = new google.visualization.ColumnChart( document.getElementById( 'accuracy_div' ) );
  chart.draw( data, options  );

}

function drawQuestionTypeTablePerStudent() {
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Topic' );
  data.addColumn( 'number', 'Attempts' );
  data.addColumn( 'number', 'Correct' );
  data.addColumn( 'number', 'Accuracy' );
  data.addColumn( 'string', 'Concepts' );
  for ( row = 0; row < questionTypeIDs.length; row++ ) {
    var code = questionTypeIDs[ row ];
    var subtopicID = code.split( "t" )[ 0 ];

    var topic = getSubtopicName( subtopicID );
    var type = "t" + code.split( "t" )[ 1 ];
    var attempts = questionTypeAttempted[ code ];
    var correct = questionTypeCorrect[ code ];
    var concepts = questionTypeNames[ code ].toString();

    if ( correct == null ) {
      correct = 0;
    }
    var accuracy = Math.round( (correct / attempts) * 1000 ) / 10;
    if ( attempts == null || attempts == 0 ) {
      attempts = 0;
      accuracy = 0;
    }

    if ( attempts != 0 ) {
      data.addRow( [ topic, attempts, correct, accuracy, concepts ] );
    }
  }
  var options = {
    'title'    : 'Question Types',
    'width'    : $( window ).width() * 0.9,
    'cssClassNames': GoogleTableCSSClassNames,
    'allowHtml': true
  };

  var percentformatter = new google.visualization.NumberFormat( { suffix: ' %' } );
  percentformatter.format( data, 3 );

  var formatter = new google.visualization.ColorFormat();
  formatter.addRange( '0', '40', 'black', '#f56151' );
  formatter.addRange( '40', '60', 'black', '#fd9c78' );
  formatter.addRange( '60', '80', 'black', '#ffe84e' );
  formatter.addRange( '80', '101', 'black', '#55cc88' );
  formatter.format( data, 3 ); // Apply formatter to second column

  var chart = new google.visualization.Table( document.getElementById( 'questiontype_div_per_student' ) );
  chart.draw( data, options );
}

function drawQuestionTypeTablePerClass() {
  
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Topic' );
  data.addColumn( 'number', ' > ' + tiers[0] + " % " );
  data.addColumn( 'number', ' > ' + tiers[1] + " % " );
  data.addColumn( 'number', ' < ' + tiers[1] + " % " );
  data.addColumn( 'string', 'Concepts' );
  for ( row = 0; row < questionTypeIDs.length; row++ ) {
    var code = questionTypeIDs[ row ];
    var subtopicID = code.split( "t" )[ 0 ];

    var accTier1 = 0;
    var accTier2 = 0;
    var accTier3 = 0;

    var topic = getSubtopicName( subtopicID );
    var type = "t" + code.split( "t" )[ 1 ];
    for(var i = 0 ; i < studentIDs.length ; i++) {
      var attempts = questionTypeAttemptedPerChild [ code ] [ studentIDs[i] ];
      var correct = questionTypeCorrectPerChild[ code ][ studentIDs[i] ];
      if ( correct == null ) {
        correct = 0;
      }
      var accuracy = Math.round( (correct / attempts) * 1000 ) / 10;
      if ( attempts == null || attempts == 0 ) {
        attempts = 0;
        accuracy = 0;
      }
      if(attempts == 0) {
        continue;
      }
      if (accuracy >= tiers[0]) {
        accTier1++;
      } else if(accuracy > tiers[1]) {
        accTier2++;
      } else {
        accTier3++;
      }
    }
    
    var concepts = questionTypeNames[ code ].toString();
    
    data.addRow( [ topic, {v: accTier1, f: '<div onclick="typeTableClicked(\'' + code + '\',0)">' + accTier1 + '</div>'},{v: accTier2, f: '<div onclick="typeTableClicked(\'' + code + '\',1)">' + accTier2 + '</div>'},{v: accTier3, f: '<div onclick="typeTableClicked(\'' + code + '\',2)">' + accTier3 + '</div>'}, concepts ] );
  }
  var options = {
    'title'    : 'Question Types',
    'width'    : $( window ).width() * 0.9,
    'cssClassNames': GoogleTableCSSClassNames,
    'allowHtml': true
  };

  var formatterTop = new google.visualization.ColorFormat();
  formatterTop.addRange( (studentIDs.length*0.7).toString(), (studentIDs.length+1).toString(), 'black', '#55cc88' );
  formatterTop.format( data, 1 ); 

  var formatterBottom = new google.visualization.ColorFormat();
  formatterBottom.addRange( (studentIDs.length*0.4).toString(), (studentIDs.length+1).toString(), 'black', '#f56151' );
  formatterBottom.format( data, 3 ); 

  var chart = new google.visualization.Table( document.getElementById( 'questiontype_div_per_class' ) );
  chart.draw( data, options );
}

function typeTableClicked(code, tierSelected) {
  var studentsSelected = new Array();
  for(var i = 0 ; i < studentIDs.length ; i++) {
      var attempts = questionTypeAttemptedPerChild [ code ] [ studentIDs[i] ];
      var correct = questionTypeCorrectPerChild[ code ][ studentIDs[i] ];
      if ( correct == null ) {
        correct = 0;
      }
      var accuracy = Math.round( (correct / attempts) * 1000 ) / 10;
      if ( attempts == null || attempts == 0 ) {
        attempts = 0;
        accuracy = 0;
      }
      if(attempts == 0) {
        continue;
      }

      if(tierSelected == 0) {
        if (accuracy >= tiers[0]) {
          studentsSelected.push(studentIDs[i]);
        }
      } else if(tierSelected == 1) {
        if (accuracy < tiers[0] && accuracy > tiers[1]) {
          studentsSelected.push(studentIDs[i]);
        }
      } else {
        if (accuracy < tiers[1]) {
          studentsSelected.push(studentIDs[i]);
        }
      }
    }
}

function drawStudentTable() {
  var data = new google.visualization.DataTable();
  data.addColumn( 'string', 'Name' );
  data.addColumn( 'number', 'Attempts' );
  data.addColumn( 'number', 'Correct' );
  data.addColumn( 'number', 'Accuracy' );
  for ( row = 0; row < studentNames.length; row++ ) {
    var name = studentNames[ row ];
    var attempts = attemptsPerChild[ studentIDs[ row ] ];
    var correct = correctPerChild[ studentIDs[ row ] ];
    if ( correct == null ) {
      correct = 0;
    }
    var accuracy = correct / attempts;//Math.round((correct / attempts) * 1000)/10;
    if ( attempts == null || attempts == 0 ) {
      attempts = 0;
      accuracy = 0;
    }
    data.addRow( [ name, attempts, correct, accuracy ] );
  }

  var percentformatter = new google.visualization.NumberFormat( { pattern: '##.# %' } );
  percentformatter.format( data, 3 );

  var formatter = new google.visualization.ColorFormat();
  formatter.addRange( '0', '0.4', 'black', '#f56151' );
  formatter.addRange( '0.4', '0.6', 'black', '#fd9c78' );
  formatter.addRange( '0.6', '0.8', 'black', '#ffe84e' );
  formatter.addRange( '0.8', '1.1', 'black', '#55cc88' );
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
  var A = [['StudentID','AssessmentItemID','Correct','Date','Difficulty','TimeStarted','TimeTaken','PlayTime','RandomSeed','MissionData']];

  for(var j=0; j<items.length; j++){
      var studentID = items[j].StudentID.S;
      var AssessmentItemID = items[j].AssessmentItemID.S;
      var Correct = items[j].Correct.BOOL.toString();
      var Difficulty = items[j].Difficulty.N.toString();
      var TimeStarted = items[j].TimeStarted.S;
      var TimeTaken = items[j].TimeTaken.N.toString();
      var date = items[j].Date.S;
      var PlayTime = "0";
      var RandomSeed = "0";
      var missionField = "";

      if(items[j].PlayTime != null) {
        PlayTime = items[j].PlayTime.S;
      }
      if(items[j].RandomSeed != null) {
        RandomSeed = items[j].RandomSeed.N.toString();
      }
      if(items[j].MissionData != null) {
        missionField = items[j].MissionData.S;
      }

      A.push([studentID, AssessmentItemID, Correct, date, Difficulty, TimeStarted, TimeTaken, PlayTime, RandomSeed]);
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
  a.download    = 'Analytics.csv';

  console.log("Exporting CSV");
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
