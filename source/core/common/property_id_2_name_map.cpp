//  
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// property_id_2_name_map.cpp: implemenation of mapping id to its name.
//

#include <unordered_map>

#include "stdafx.h"
#include "speechapi_cxx_properties.h"
#include "property_id_2_name_map.h"

namespace Microsoft {
namespace CognitiveServices {
namespace Speech {
namespace Impl {

const char* GetPropertyName(const PropertyId& id)
{
    switch (id)
    {
    case PropertyId::SpeechServiceConnection_Key: return "SPEECH-SubscriptionKey";
    case PropertyId::SpeechServiceConnection_Endpoint: return "SPEECH-Endpoint";
    case PropertyId::SpeechServiceConnection_Region: return "SPEECH-Region";
    case PropertyId::SpeechServiceAuthorization_Token: return "SPEECH-AuthToken";
    case PropertyId::SpeechServiceAuthorization_Type: return "SpeechServiceAuthorization_Type";
    case PropertyId::SpeechServiceConnection_EndpointId: return "SPEECH-ModelId";
    case PropertyId::SpeechServiceConnection_TranslationToLanguages: return "TRANSLATION-ToLanguages";
    case PropertyId::SpeechServiceConnection_TranslationVoice: return "TRANSLATION-Voice";
    case PropertyId::SpeechServiceConnection_TranslationFeatures: return "TRANSLATION-Features";
    case PropertyId::SpeechServiceConnection_IntentRegion: return "INTENT-region";
    case PropertyId::SpeechServiceConnection_RecoMode: return "SPEECH-RecoMode";
    case PropertyId::SpeechServiceConnection_RecoLanguage: return "SPEECH-RecoLanguage";
    case PropertyId::Speech_SessionId: return "SessionId";
    case PropertyId::SpeechServiceResponse_RequestDetailedResultTrueFalse: return "SpeechServiceResponse_RequestDetailedResultTrueFalse";
    case PropertyId::SpeechServiceResponse_RequestProfanityFilterTrueFalse: return "SpeechServiceResponse_RequestProfanityFilterTrueFalse";
    case PropertyId::SpeechServiceResponse_JsonResult: return "RESULT-Json";
    case PropertyId::LanguageUnderstandingServiceResponse_JsonResult: return "RESULT-LanguageUnderstandingJson";
    case PropertyId::SpeechServiceResponse_JsonErrorDetails: return "RESULT-ErrorDetails";
    case PropertyId::CancellationDetails_Reason: return "CancellationDetails_Reason";
    case PropertyId::CancellationDetails_ReasonText: return "CancellationDetails_ReasonText";
    case PropertyId::CancellationDetails_ReasonDetailedText: return "CancellationDetails_ReasonDetailedText";
    default:
        LogError("undefined PropertyId of %d", static_cast<int>(id));
        SPX_THROW_ON_FAIL(SPXERR_INVALID_ARG);
        return "";
    }

}
}}}} // Microsoft::CognitiveServices::Speech::Impl