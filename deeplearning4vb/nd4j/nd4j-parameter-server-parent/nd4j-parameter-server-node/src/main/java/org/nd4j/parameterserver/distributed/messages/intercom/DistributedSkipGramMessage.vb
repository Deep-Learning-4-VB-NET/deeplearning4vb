Imports System
Imports NonNull = lombok.NonNull
Imports BaseVoidMessage = org.nd4j.parameterserver.distributed.messages.BaseVoidMessage
Imports DistributedMessage = org.nd4j.parameterserver.distributed.messages.DistributedMessage
Imports SkipGramRequestMessage = org.nd4j.parameterserver.distributed.messages.requests.SkipGramRequestMessage

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

Namespace org.nd4j.parameterserver.distributed.messages.intercom

	<Obsolete, Serializable>
	Public Class DistributedSkipGramMessage
		Inherits BaseVoidMessage
		Implements DistributedMessage

		' learning rate for this sequence
		Protected Friend alpha As Double

		' current word & lastWord
		Protected Friend w1 As Integer
		Protected Friend w2 As Integer

		' following fields are for hierarchic softmax
		' points & codes for current word
		Protected Friend points() As Integer
		Protected Friend codes() As SByte

		Protected Friend negSamples As Short

		Protected Friend nextRandom As Long


		Protected Friend Sub New()
			MyBase.New(23)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public DistributedSkipGramMessage(@NonNull SkipGramRequestMessage message)
		Public Sub New(ByVal message As SkipGramRequestMessage)
			Me.New()

			Me.w1 = message.getW1()
			Me.w2 = message.getW2()
			Me.points = message.getPoints()
			Me.codes = message.getCodes()

			Me.negSamples = message.getNegSamples()
			Me.nextRandom = message.getNextRandom()
		End Sub

		Public Overrides Sub processMessage()

		End Sub
	End Class

End Namespace