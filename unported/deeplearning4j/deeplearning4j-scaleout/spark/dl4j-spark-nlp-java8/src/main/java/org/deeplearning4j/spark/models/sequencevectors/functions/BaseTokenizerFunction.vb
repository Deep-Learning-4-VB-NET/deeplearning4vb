Imports System
Imports NonNull = lombok.NonNull
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports DL4JClassLoading = org.deeplearning4j.common.config.DL4JClassLoading
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports TokenPreProcess = org.deeplearning4j.text.tokenization.tokenizer.TokenPreProcess
Imports TokenizerFactory = org.deeplearning4j.text.tokenization.tokenizerfactory.TokenizerFactory

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

Namespace org.deeplearning4j.spark.models.sequencevectors.functions


	<Serializable>
	Public MustInherit Class BaseTokenizerFunction
		Protected Friend configurationBroadcast As Broadcast(Of VectorsConfiguration)

		<NonSerialized>
		Protected Friend tokenizerFactory As TokenizerFactory
		<NonSerialized>
		Protected Friend tokenPreprocessor As TokenPreProcess

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: protected BaseTokenizerFunction(@NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> configurationBroadcast)
		Protected Friend Sub New(ByVal configurationBroadcast As Broadcast(Of VectorsConfiguration))
			Me.configurationBroadcast = configurationBroadcast
		End Sub

		Protected Friend Overridable Sub instantiateTokenizerFactory()
			Dim tfClassName As String = Me.configurationBroadcast.getValue().getTokenizerFactory()
			Dim tpClassName As String = Me.configurationBroadcast.getValue().getTokenPreProcessor()

			If tfClassName IsNot Nothing AndAlso tfClassName.Length > 0 Then
				tokenizerFactory = DL4JClassLoading.createNewInstance(tfClassName)

				If tpClassName IsNot Nothing AndAlso tpClassName.Length > 0 Then
					tokenPreprocessor = DL4JClassLoading.createNewInstance(tpClassName)
				End If

				If tokenPreprocessor IsNot Nothing Then
					tokenizerFactory.TokenPreProcessor = tokenPreprocessor
				End If
			Else
				Throw New Exception("TokenizerFactory wasn't defined.")
			End If
		End Sub
	End Class

End Namespace