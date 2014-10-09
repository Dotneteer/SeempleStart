delete from [Email].[Template]

delete from [Email].[EmailSent]
delete from [Email].[EmailToSend]
delete from [Email].[EmailFailed]

insert into [Email].[Template](Id, Subject, Body)
values ('SelfEvalCompleted', N'Self evaluation completed',
N'<!DOCTYPE html>
<html>
<head>
    <title>Self evaluation completed</title>
    <style>
        p {
            font-family: monospace
        }
    </style>
</head>
<body>
  <p>Dear User,</p>
  <br/>
  <p>this is an automatically generated e-mail, from our Perform@@work system.</p>
  <p>Please be informed that <b>{{FullName}} has completed his/her Self-evaluation.</b></p>
  <p>To enable the evaluation move forward you are kindly asked to LOG IN THE SYSTEM AND SET UP AN EVALUATION MEETING.</p>
  <p>This meeting has to be registered/accepted in the system, from where an automatic alert will be sent to the other party.</p>
  <br/>
  <p>Thank you</p>
  <br/>
  <p>Your HR Team</p>
  <p>************</p>
</body>
</html>'),
('MeetingProposed', N'Evaluation meeting proposed',
N'<!DOCTYPE html>
<html>
<head>
    <title>Self evaluation completed</title>
    <style>
        p {
            font-family: monospace
        }
    </style>
</head>
<body>
  <p>Dear User,</p>
  <br/>
  <p>this is an automatically generated e-mail, from our Perform@@work system.</p>
  <p>Please be informed that <b>{{FullName}} has proposed an evaluation meeting.</b></p>
  <p>To enable the evaluation move forward you are kindly asked to <a href="http://perform.euromedic.com"> LOG IN THE SYSTEM AND ACCEPT THIS EVALUATION MEETING.</a></p>
  <p>This meeting has to be registered/accepted in the system, from where an automatic alert will be sent to the other party.</p>
  <br/>
  <p>Thank you</p>
  <br/>
  <p>Your HR Team</p>
  <p>************</p>
</body>
</html>'),
('MeetingAccepted', N'Evaluation meeting accepted',
N'<!DOCTYPE html>
<html>
<head>
    <title>Self evaluation completed</title>
    <style>
        p {
            font-family: monospace
        }
    </style>
</head>
<body>
  <p>Dear User,</p>
  <br/>
  <p>this is an automatically generated e-mail, from our Perform@@work system.</p>
  <p>Please be informed that <b>{{FullName}} has accepted the proposed evaluation meeting.</b></p>
  <br/>
  <p>Thank you</p>
  <br/>
  <p>Your HR Team</p>
  <p>************</p>
</body>
</html>'),
('MeetingRejected', N'Evaluation meeting rejected',
N'<!DOCTYPE html>
<html>
<head>
    <title>Self evaluation completed</title>
    <style>
        p {
            font-family: monospace
        }
    </style>
</head>
<body>
  <p>Dear User,</p>
  <br/>
  <p>this is an automatically generated e-mail, from our Perform@@work system.</p>
  <p>Please be informed that <b>{{FullName}} has rejected the proposed evaluation meeting.</b></p>
  <p>To enable the evaluation move forward you are kindly asked to LOG IN THE SYSTEM AND PROPOSE A NEW DATE FOR THE EVALUATION MEETING.</p>
  <p>This meeting has to be registered/accepted in the system, from where an automatic alert will be sent to the other party.</p>
  <br/>
  <p>Thank you</p>
  <br/>
  <p>Your HR Team</p>
  <p>************</p>
</body>
</html>'),
('EvalCommented', N'Evaluation meeting commented',
N'<!DOCTYPE html>
<html>
<head>
    <title>Self evaluation completed</title>
    <style>
        p {
            font-family: monospace
        }
    </style>
</head>
<body>
  <p>Dear User,</p>
  <br/>
  <p>this is an automatically generated e-mail, from our Perform@@work system.</p>
  <p>Please be informed that <b>{{FullName}} has left a comment at the end of the finished cycle of his/her evaluation.</b></p>
  <p>The text of this comment:</p>
  <blockquote>{{Comment}}</blockquote>
  <br/>
  <p>Thank you</p>
  <br/>
  <p>Your HR Team</p>
  <p>************</p>
</body>
</html>'),
('AssessmentClosed', N'Assessment closed',
N'<!DOCTYPE html>
<html>
<head>
    <title>Your assessment has been successfully closed</title>
    <style>
        p {
            font-family: monospace
        }
    </style>
</head>
<body>
  <p>Dear {{EmployeeName}},</p>
  <br/>
  <p>this is an automatically generated e-mail, from our Perform@@work system.</p>
  <p>Thank you for participating in the performance appraisal process. <b>Your assessment has been successfully closed</b>.
     We trust that the discussions between you and your manager will contribute to your cooperation in the future.</p>
  <br/>
  <p>In case you wish to LEAVE A COMMENT (on the evaluation, if you had a point where you didn’t agree, you have some suggestions, etc) 
     you can do this in the system. The opportunity to comment is open for a week’s time from today. 
     To leave a comment please log in to the system and look for "EVALUATION MEETING FEEDBACK" tab within the "Self-evaluation" menu.</p>
  <br/>
  <p>Thank you for your time and see you again in the next evaluation campaign!</p>
  <br/>
  <p>Your HR Team</p>
  <p>************</p>
</body>
</html>')
