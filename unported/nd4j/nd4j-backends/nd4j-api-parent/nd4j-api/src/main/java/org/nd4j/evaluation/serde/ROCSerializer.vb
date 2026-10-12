Imports ROC = org.nd4j.evaluation.classification.ROC
Imports JsonGenerator = org.nd4j.shade.jackson.core.JsonGenerator
Imports JsonSerializer = org.nd4j.shade.jackson.databind.JsonSerializer
Imports SerializerProvider = org.nd4j.shade.jackson.databind.SerializerProvider
Imports TypeSerializer = org.nd4j.shade.jackson.databind.jsontype.TypeSerializer

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.evaluation.serde


	Public Class ROCSerializer
		Inherits JsonSerializer(Of ROC)

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serialize(org.nd4j.evaluation.classification.ROC roc, org.nd4j.shade.jackson.core.JsonGenerator jsonGenerator, org.nd4j.shade.jackson.databind.SerializerProvider serializerProvider) throws java.io.IOException
		Public Overrides Sub serialize(ByVal roc As ROC, ByVal jsonGenerator As JsonGenerator, ByVal serializerProvider As SerializerProvider)
			Dim empty As Boolean = roc.getExampleCount() = 0

			If roc.isExact() AndAlso Not empty Then
				'For exact ROC implementation: force AUC and AUPRC calculation, so result can be stored in JSON, such
				'that we have them once deserialized.
				'Due to potentially huge size, exact mode doesn't store the original predictions in JSON
				roc.calculateAUC()
				roc.calculateAUCPR()
			End If
			jsonGenerator.writeNumberField("thresholdSteps", roc.getThresholdSteps())
			jsonGenerator.writeNumberField("countActualPositive", roc.getCountActualPositive())
			jsonGenerator.writeNumberField("countActualNegative", roc.getCountActualNegative())
			jsonGenerator.writeObjectField("counts", roc.getCounts())
			If Not empty Then
				jsonGenerator.writeNumberField("auc", roc.calculateAUC())
				jsonGenerator.writeNumberField("auprc", roc.calculateAUCPR())
			End If
			If roc.isExact() AndAlso Not empty Then
				'Store ROC and PR curves only for exact mode... they are redundant + can be calculated again for thresholded mode
				jsonGenerator.writeObjectField("rocCurve", roc.RocCurve)
				jsonGenerator.writeObjectField("prCurve", roc.PrecisionRecallCurve)
			End If
			jsonGenerator.writeBooleanField("isExact", roc.isExact())
			jsonGenerator.writeNumberField("exampleCount", roc.getExampleCount())
			jsonGenerator.writeBooleanField("rocRemoveRedundantPts", roc.isRocRemoveRedundantPts())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void serializeWithType(org.nd4j.evaluation.classification.ROC value, org.nd4j.shade.jackson.core.JsonGenerator gen, org.nd4j.shade.jackson.databind.SerializerProvider serializers, org.nd4j.shade.jackson.databind.jsontype.TypeSerializer typeSer) throws java.io.IOException
		Public Overrides Sub serializeWithType(ByVal value As ROC, ByVal gen As JsonGenerator, ByVal serializers As SerializerProvider, ByVal typeSer As TypeSerializer)
			typeSer.writeTypePrefixForObject(value, gen)
			serialize(value, gen, serializers)
			typeSer.writeTypeSuffixForObject(value, gen)
		End Sub
	End Class

End Namespace