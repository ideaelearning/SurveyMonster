-2026 seçimi vatandaşa soruyoruz-

1-Akp seçimi kazanır mı? 
-evet
-hayır

2-Chp seçimi kazanır mı?
-evet
-hayır


{{baseUrl}}/api/surveys/survey
Request:
{
    "TenantId": "1",
    "SurveyTypeId": 1,
    "SurveyQuestionBankId": null,
    "Name": "2026 seçimi vatandaşa soruyoruz",
    "InformationText": "herşey vatandaş için...",
    "isStatic": false,
    "ExpireDate": "2026-12-12",
    "State": 1
}
Response:
{
    "data": {
        "id": 2544
    }
}


{{baseUrl}}/api/surveys/surveyQuestion
Request:
{
    "Text": "Akp seçimi kazanır mı?",
    "SurveyQuestionTypeId": 1,
    "SurveyCategoryId": 1,
    "State": 1,
    "OrganizationUnitId": null,
    "Discriminator":"SurveyQuestion"
}
Response:
{
    "data": {
        "id": 2544
    }
}


{{baseUrl}}/api/surveys/surveyQuestionOption
Request:
{
            "TenantId": 1,
            "SurveyQuestionId": 2543,
            "Text": "Hayır",
            "Value": 2
          
}
Response:
{
    "data": {
        "id": 2544
    }
}


{{baseUrl}}/api/surveys/surveyQuestionOrder
Request:
{
            "TenantId": null,
            "SurveyId": 534,
            "SurveyQuestionId": 2544,
            "Order": 2
}
Response:
{
    "data": {
        "id": 2544
    }
}

Ankete istek attığım tüm anket bilgilerini aldığım api
{{baseUrl}}/api/Surveys/survey/534
Response:
{
    "data": {
        "id": 534,
        "surveyTypeId": 1,
        "surveyCategoryId": null,
        "surveyQuestionBankId": null,
        "name": "2026 seçimi vatandaşa soruyoruz",
        "informationText": "herşey vatandaş için...",
        "showChapters": false,
        "isTeamplate": false,
        "isStatic": false,
        "surveyType": {
            "id": 1,
            "name": "Değerlendirme Anketi",
            "description": "",
            "isDefault": false,
            "isStatic": false,
            "tenantId": 5
        },
        "surveyQuestionBank": null,
        "surveyQuestions": null,
        "surveyChapters": null,
        "surveySurveyQuestionOrders": [
            {
                "id": 3225,
                "tenantId": null,
                "surveyId": 534,
                "surveyQuestionId": 2543,
                "order": 1,
                "surveyChapterId": null,
                "surveyQuestion": {
                    "id": 2543,
                    "useNotMaxLength": false,
                    "tenantId": null,
                    "text": "Akp seçimi kazanır mı?",
                    "surveyQuestionTypeId": 1,
                    "surveyQuestionType": null,
                    "surveyQuestionTypeText": "",
                    "surveyCategoryId": 1,
                    "surveyCategory": null,
                    "surveyCategoryText": "",
                    "stateId": 1,
                    "state": 1,
                    "surveySurveyQuestionOptions": [
                        {
                            "id": 11829,
                            "tenantId": 1,
                            "surveyQuestionId": 2543,
                            "text": "Evet",
                            "value": 1,
                            "isOther": false,
                            "verificationTypeId": null,
                            "verificationType": null,
                            "numberOfSelection": 0,
                            "optionColor": null,
                            "emptyOfSelection": 0
                        },
                        {
                            "id": 11830,
                            "tenantId": 1,
                            "surveyQuestionId": 2543,
                            "text": "Hayır",
                            "value": 2,
                            "isOther": false,
                            "verificationTypeId": null,
                            "verificationType": null,
                            "numberOfSelection": 0,
                            "optionColor": null,
                            "emptyOfSelection": 0
                        }
                    ]
                },
                "surveyChapter": null
            },
            {
                "id": 3226,
                "tenantId": null,
                "surveyId": 534,
                "surveyQuestionId": 2544,
                "order": 2,
                "surveyChapterId": null,
                "surveyQuestion": {
                    "id": 2544,
                    "useNotMaxLength": false,
                    "tenantId": null,
                    "text": "CHP yine seçimi kaybeder mi?",
                    "surveyQuestionTypeId": 1,
                    "surveyQuestionType": null,
                    "surveyQuestionTypeText": "",
                    "surveyCategoryId": 1,
                    "surveyCategory": null,
                    "surveyCategoryText": "",
                    "stateId": 1,
                    "state": 1,
                    "surveySurveyQuestionOptions": [
                        {
                            "id": 11831,
                            "tenantId": 1,
                            "surveyQuestionId": 2544,
                            "text": "Hayır",
                            "value": 2,
                            "isOther": false,
                            "verificationTypeId": null,
                            "verificationType": null,
                            "numberOfSelection": 0,
                            "optionColor": null,
                            "emptyOfSelection": 0
                        },
                        {
                            "id": 11832,
                            "tenantId": 1,
                            "surveyQuestionId": 2544,
                            "text": "Evet",
                            "value": 1,
                            "isOther": false,
                            "verificationTypeId": null,
                            "verificationType": null,
                            "numberOfSelection": 0,
                            "optionColor": null,
                            "emptyOfSelection": 0
                        }
                    ]
                },
                "surveyChapter": null
            }
        ],
        "surveyAssignments": null,
        "expireDate": "2026-12-12T00:00:00",
        "state": 1,
        "state2": null
    }
}

{{baseUrl}}/api/surveys/surveyAssignment
Request:
{
    "surveyId": 534,
    "Discriminator": "SurveyAssignment",
    "startDate": "2020-05-05",
    "endDate": "2026-12-12",
    "EventCategoryId": 7,
    "title": "atama başlığı",
    "Imperative": false,
    "SurveyMaxTakeCount": 1111,
    "ExamSecurityType":0
}
Response:
{
    "data": {
        "id": 30909
    }
}


{{baseUrl}}/api/surveys/surveyAssignmentTaker
Request:
{
    "surveyAssignmentId": 30909,
    "userId": 74805,
    "Discriminator":"SurveyAssignmentTaker"
}
Response:
{
    "data": {
        "id": 951163
    }
}

request : {{baseUrl}}/api/surveys/surveyAssignmentTakerEntry
{
    "SurveyAssignmentTakerId":951163,
    "StartDate":"2020-01-01",
    "FinishDate":"2026-12-12",
    "SurveyState":1
}
 
response:
{
    "data": {
        "id": 23267
    }
}
 
request: {{baseUrl}}/api/surveys/surveyAssignmentTakerEntryGivenAnswer
{
    "SurveyAssignmentTakerEntryId": 16361,
    "SurveyQuestionId": 2544,
    "Answer": "11831",
    "IsEmpty": false,
    "TenantId": 1
}
 
{
    "data": {
        "id": 101130
    }
}

{{baseUrl}}/api/identity/auth/login
Request:
{
    "username": "cem.kurt@ideaelearning.net",
    "password": "Idea.123!",
    "language": "tr"
}
Response:
{
    "data": {
        "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyaWQiOiI3NDgwNSIsImZ1bGxuYW1lIjoiQ2VtIEt1cnQiLCJhY2NvdW50dHlwZSI6Ik1hbmFnZXIiLCJlbWFpbCI6ImNlbS5rdXJ0QGlkZWFlbGVhcm5pbmcubmV0Iiwicm9sZSI6Ik1hbmFnZXIiLCJ0ZW5hbnRpZCI6IjEiLCJzdGFmZmlkIjoiMSIsInN0dWRlbnRpZCI6IjEiLCJpYXQiOiIxNzYyNTIzODg1IiwiZXhwIjoxNzYyNzgzMDg1LCJpc3MiOiJMbXNSZWRlc2lnbiIsImF1ZCI6Ikxtc1JlZGVzaWduIn0.-AZDw4cmj96UistqkBFyYEmAy6rbA3JkVxm6ovSqiLk",
        "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyaWQiOiI3NDgwNSIsInRlbmFudGlkIjoiMSIsImV4cCI6MTc2MzA0MjI4NX0.jbvg1icIL_B39FhsCqsz05edtQgqHzeTJTPKRgmBTuY"
    }
}