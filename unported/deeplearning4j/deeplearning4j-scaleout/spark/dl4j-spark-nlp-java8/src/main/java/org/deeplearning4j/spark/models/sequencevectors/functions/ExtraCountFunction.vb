Imports NonNull = lombok.NonNull
Imports Accumulator = org.apache.spark.Accumulator
Imports [Function] = org.apache.spark.api.java.function.Function
Imports org.deeplearning4j.models.sequencevectors.sequence
Imports SequenceElement = org.deeplearning4j.models.sequencevectors.sequence.SequenceElement
Imports org.deeplearning4j.spark.models.sequencevectors.primitives
Imports org.nd4j.common.primitives

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

	Public Class ExtraCountFunction(Of T As org.deeplearning4j.models.sequencevectors.sequence.SequenceElement)
		Implements [Function](Of Sequence(Of T), Pair(Of Sequence(Of T), Long))

		Protected Friend accumulator As Accumulator(Of ExtraCounter(Of Long))
		Protected Friend fetchLabels As Boolean

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public ExtraCountFunction(@NonNull Accumulator<org.deeplearning4j.spark.models.sequencevectors.primitives.ExtraCounter<Long>> accumulator, boolean fetchLabels)
		Public Sub New(ByVal accumulator As Accumulator(Of ExtraCounter(Of Long)), ByVal fetchLabels As Boolean)
			Me.accumulator = accumulator
			Me.fetchLabels = fetchLabels
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.nd4j.common.primitives.Pair<org.deeplearning4j.models.sequencevectors.sequence.Sequence<T>, Long> call(org.deeplearning4j.models.sequencevectors.sequence.Sequence<T> sequence) throws Exception
		Public Overrides Function [call](ByVal sequence As Sequence(Of T)) As Pair(Of Sequence(Of T), Long)
			' since we can't be 100% sure that sequence size is ok itself, or it's not overflow through int limits, we'll recalculate it.
			' anyway we're going to loop through it for elements frequencies
			Dim localCounter As New ExtraCounter(Of Long)()
			Dim seqLen As Long = 0

			For Each element As T In sequence.getElements()
				If element Is Nothing Then
					Continue For
				End If

				' FIXME: hashcode is bad idea here. we need Long id
				localCounter.incrementCount(element.getStorageId(), 1.0f)
				seqLen += 1
			Next element

			' FIXME: we're missing label information here due to shallow vocab mechanics
			If sequence.getSequenceLabels() IsNot Nothing Then
				For Each label As T In sequence.getSequenceLabels()
					localCounter.incrementCount(label.getStorageId(), 1.0f)
				Next label
			End If

			localCounter.buildNetworkSnapshot()

			accumulator.add(localCounter)

			Return Pair.makePair(sequence, seqLen)
		End Function
	End Class

End Namespace