Imports System
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports [Function] = org.apache.spark.api.java.function.Function
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports VectorsConfiguration = org.deeplearning4j.models.embeddings.loader.VectorsConfiguration
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore
Imports org.deeplearning4j.spark.models.sequencevectors.export
Imports VoidParameterServer = org.nd4j.parameterserver.distributed.VoidParameterServer
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class DistributedFunction<T extends org.deeplearning4j.models.sequencevectors.sequence.SequenceElement> implements org.apache.spark.api.java.function.@Function<T, org.deeplearning4j.spark.models.sequencevectors.export.ExportContainer<T>>
	Public Class DistributedFunction(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements [Function](Of T, ExportContainer(Of T))

		Protected Friend configurationBroadcast As Broadcast(Of VoidConfiguration)
		Protected Friend vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration)
		Protected Friend shallowVocabBroadcast As Broadcast(Of VocabCache(Of ShallowSequenceElement))

		<NonSerialized>
		Protected Friend shallowVocabCache As VocabCache(Of ShallowSequenceElement)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedFunction(@NonNull Broadcast<org.nd4j.parameterserver.distributed.conf.VoidConfiguration> configurationBroadcast, @NonNull Broadcast<org.deeplearning4j.models.embeddings.loader.VectorsConfiguration> vectorsConfigurationBroadcast, @NonNull Broadcast<org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement>> shallowVocabBroadcast)
		Public Sub New(ByVal configurationBroadcast As Broadcast(Of VoidConfiguration), ByVal vectorsConfigurationBroadcast As Broadcast(Of VectorsConfiguration), ByVal shallowVocabBroadcast As Broadcast(Of VocabCache(Of ShallowSequenceElement)))
			Me.configurationBroadcast = configurationBroadcast
			Me.vectorsConfigurationBroadcast = vectorsConfigurationBroadcast
			Me.shallowVocabBroadcast = shallowVocabBroadcast
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.deeplearning4j.spark.models.sequencevectors.export.ExportContainer<T> call(T word) throws Exception
		Public Overrides Function [call](ByVal word As T) As ExportContainer(Of T)
			If shallowVocabCache Is Nothing Then
				shallowVocabCache = shallowVocabBroadcast.getValue()
			End If

			Dim container As New ExportContainer(Of T)()

			Dim reduced As ShallowSequenceElement = shallowVocabCache.tokenFor(word.getStorageId())
			word.setIndex(reduced.Index)

			container.setElement(word)
			container.setArray(VoidParameterServer.Instance.getVector(reduced.Index))

			Return container
		End Function
	End Class

End Namespace