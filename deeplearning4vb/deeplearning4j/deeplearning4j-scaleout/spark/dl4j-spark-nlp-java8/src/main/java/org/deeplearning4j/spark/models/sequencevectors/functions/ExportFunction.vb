Imports NonNull = lombok.NonNull
Imports VoidFunction = org.apache.spark.api.java.function.VoidFunction
Imports Broadcast = org.apache.spark.broadcast.Broadcast
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports ShallowSequenceElement = org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement
Imports org.deeplearning4j.models.word2vec.wordstore

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

	Public Class ExportFunction(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements VoidFunction(Of T)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExportFunction(org.apache.spark.broadcast.Broadcast<org.deeplearning4j.models.word2vec.wordstore.VocabCache<org.deeplearning4j.models.sequencevectors.sequence.ShallowSequenceElement>> vocabCacheBroadcast, @NonNull String hdfsFilePath)
		Public Sub New(ByVal vocabCacheBroadcast As Broadcast(Of VocabCache(Of ShallowSequenceElement)), ByVal hdfsFilePath As String)

		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void call(T t) throws Exception
		Public Overrides Sub [call](ByVal t As T)

		End Sub
	End Class

End Namespace