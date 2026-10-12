Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Data = lombok.Data
Imports Base64 = org.apache.commons.codec.binary.Base64
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature

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

Namespace org.deeplearning4j.models.embeddings.loader


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class VectorsConfiguration implements java.io.Serializable
	<Serializable>
	Public Class VectorsConfiguration

		' word2vec params
		Private minWordFrequency As Integer = 5
		Private learningRate As Double = 0.025
		Private minLearningRate As Double = 0.0001
		Private layersSize As Integer = 200
		Private useAdaGrad As Boolean = False
		Private batchSize As Integer = 512
		Private iterations As Integer = 1
		Private epochs As Integer = 1
		Private window As Integer = 5
		Private seed As Long
		Private negative As Double = 0.0R
		Private useHierarchicSoftmax As Boolean = True
		Private sampling As Double = 0.0R
		Private learningRateDecayWords As Integer
		Private variableWindows() As Integer

		Private hugeModelExpected As Boolean = False
		Private useUnknown As Boolean = False

		Private scavengerActivationThreshold As Integer = 2000000
		Private scavengerRetentionDelay As Integer = 3

		Private elementsLearningAlgorithm As String
		Private sequenceLearningAlgorithm As String
		Private modelUtils As String

		Private tokenizerFactory As String
		Private tokenPreProcessor As String

		' this is one-off configuration value specially for NGramTokenizerFactory
		Private nGram As Integer

		Private UNK As String = "UNK"
		Private [STOP] As String = "STOP"

		Private stopList As ICollection(Of String) = New List(Of String)()

		' overall model info
		Private vocabSize As Integer

		' paravec-specific option
		Private trainElementsVectors As Boolean = True
		Private trainSequenceVectors As Boolean = True
		Private allowParallelTokenization As Boolean = False
		Private preciseWeightInit As Boolean = False

		Private preciseMode As Boolean = False

'JAVA TO VB CONVERTER NOTE: The field mapper was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared mapper_Conflict As ObjectMapper
		Private Shared ReadOnly lock As New Object()

		Private Shared Function mapper() As ObjectMapper
			If mapper_Conflict Is Nothing Then
				SyncLock lock
					If mapper_Conflict Is Nothing Then
						mapper_Conflict = New ObjectMapper()
						mapper_Conflict.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
						mapper_Conflict.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
						mapper_Conflict.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
						Return mapper_Conflict
					End If
				End SyncLock
			End If
			Return mapper_Conflict
		End Function

		Public Overridable Function toJson() As String
			Dim mapper As ObjectMapper = VectorsConfiguration.mapper()
			Try
	'            
	'                we need JSON as single line to save it at first line of the CSV model file
	'                That's ugly method, but its way more memory-friendly then loading whole 10GB json file just to create another 10GB memory array.
	'            
				Return mapper.writeValueAsString(Me)
			Catch e As org.nd4j.shade.jackson.core.JsonProcessingException
				Throw New Exception(e)
			End Try
		End Function

		Public Overridable Function toEncodedJson() As String
			Dim base64 As New Base64(Integer.MaxValue)
			Try
				Return base64.encodeAsString(Me.toJson().GetBytes(Encoding.UTF8))
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		Public Shared Function fromJson(ByVal json As String) As VectorsConfiguration
			Dim mapper As ObjectMapper = VectorsConfiguration.mapper()
			Try
				Dim ret As VectorsConfiguration = mapper.readValue(json, GetType(VectorsConfiguration))
				Return ret
			Catch e As IOException
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace