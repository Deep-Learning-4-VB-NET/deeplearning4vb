Imports Getter = lombok.Getter
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports NonNull = lombok.NonNull
Imports Setter = lombok.Setter
Imports AtomicState = org.nd4j.jita.allocator.concurrency.AtomicState
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports RateTimer = org.nd4j.jita.allocator.time.RateTimer
Imports BinaryTimer = org.nd4j.jita.allocator.time.impl.BinaryTimer

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

Namespace org.nd4j.jita.allocator.impl


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor public class NestedPoint
	Public Class NestedPoint
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @NonNull private AllocationShape shape;
		Private shape As AllocationShape
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter @NonNull private org.nd4j.jita.allocator.concurrency.AtomicState accessState;
		Private accessState As AtomicState
		Private accessTime As AtomicLong
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.jita.allocator.time.RateTimer timerShort = new org.nd4j.jita.allocator.time.impl.BinaryTimer(10, java.util.concurrent.TimeUnit.SECONDS);
		Private timerShort As RateTimer = New BinaryTimer(10, TimeUnit.SECONDS)
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.nd4j.jita.allocator.time.RateTimer timerLong = new org.nd4j.jita.allocator.time.impl.BinaryTimer(60, java.util.concurrent.TimeUnit.SECONDS);
		Private timerLong As RateTimer = New BinaryTimer(60, TimeUnit.SECONDS)


		' by default memory is UNDEFINED, and depends on parent memory chunk for now
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter @Setter private org.nd4j.jita.allocator.enums.AllocationStatus nestedStatus = org.nd4j.jita.allocator.enums.AllocationStatus.UNDEFINED;
		Private nestedStatus As AllocationStatus = AllocationStatus.UNDEFINED

		Private counter As New AtomicLong(0)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public NestedPoint(@NonNull AllocationShape shape)
		Public Sub New(ByVal shape As AllocationShape)
			Me.shape = shape
		End Sub

		''' <summary>
		''' Returns number of ticks for this point
		''' 
		''' @return
		''' </summary>
		Public Overridable ReadOnly Property Ticks As Long
			Get
				Return counter.get()
			End Get
		End Property

		''' <summary>
		''' Increments number of ticks by one
		''' </summary>
		Public Overridable Sub tick()
			accessTime.set(System.nanoTime())
			Me.counter.incrementAndGet()
		End Sub

		Public Overridable Sub tack()
			' TODO: to be implemented
			' TODO: or not
		End Sub

		Public Overrides Function Equals(ByVal o As Object) As Boolean
			If Me Is o Then
				Return True
			End If
			If o Is Nothing OrElse Me.GetType() <> o.GetType() Then
				Return False
			End If

			Dim that As NestedPoint = DirectCast(o, NestedPoint)

			Return If(shape IsNot Nothing, shape.Equals(that.shape), that.shape Is Nothing)

		End Function

		Public Overrides Function GetHashCode() As Integer
			Return If(shape IsNot Nothing, shape.GetHashCode(), 0)
		End Function
	End Class

End Namespace