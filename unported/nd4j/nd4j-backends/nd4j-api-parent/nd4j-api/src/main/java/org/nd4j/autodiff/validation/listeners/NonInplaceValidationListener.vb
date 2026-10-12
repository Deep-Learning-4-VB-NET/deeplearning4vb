Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports Getter = lombok.Getter
Imports At = org.nd4j.autodiff.listeners.At
Imports BaseListener = org.nd4j.autodiff.listeners.BaseListener
Imports Operation = org.nd4j.autodiff.listeners.Operation
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.nd4j.autodiff.validation.listeners


	Public Class NonInplaceValidationListener
		Inherits BaseListener

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static java.util.concurrent.atomic.AtomicInteger useCounter = new java.util.concurrent.atomic.AtomicInteger();
		Private Shared useCounter As New AtomicInteger()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static java.util.concurrent.atomic.AtomicInteger passCounter = new java.util.concurrent.atomic.AtomicInteger();
		Private Shared passCounter As New AtomicInteger()
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private static java.util.concurrent.atomic.AtomicInteger failCounter = new java.util.concurrent.atomic.AtomicInteger();
		Private Shared failCounter As New AtomicInteger()

		Protected Friend opInputs() As INDArray
		Protected Friend opInputsOrig() As INDArray

		Public Sub New()
			useCounter.getAndIncrement()
		End Sub

		Public Overrides Sub preOpExecution(ByVal sd As SameDiff, ByVal at As At, ByVal op As SameDiffOp, ByVal oc As OpContext)
			If op.Op.isInPlace() Then
				'Don't check inplace op
				Return
			End If
			If TypeOf op.Op Is Op Then
				Dim o As Op = DirectCast(op.Op, Op)
				If oc.getInputArray(0) Is Nothing Then
					'No input op
					Return
				ElseIf oc.getInputArray(1) Is Nothing Then
					opInputsOrig = New INDArray(){oc.getInputArray(0)}
					opInputs = New INDArray(){oc.getInputArray(0).dup()}
				Else
					opInputsOrig = New INDArray(){oc.getInputArray(0), oc.getInputArray(1)}
					opInputs = New INDArray(){oc.getInputArray(0).dup(), oc.getInputArray(1).dup()}
				End If
			ElseIf TypeOf op.Op Is DynamicCustomOp Then
				Dim arr As IList(Of INDArray) = oc.getInputArrays() ' ((DynamicCustomOp) op.getOp()).inputArguments();
				opInputs = New INDArray(arr.Count - 1){}
				opInputsOrig = New INDArray(arr.Count - 1){}
				For i As Integer = 0 To arr.Count - 1
					opInputsOrig(i) = arr(i)
					opInputs(i) = arr(i).dup()
				Next i
			Else
				Throw New System.InvalidOperationException("Unknown op type: " & op.Op.GetType())
			End If
		End Sub

		Public Overrides Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)
			If op.Op.isInPlace() Then
				'Don't check inplace op
				Return
			End If

			Dim md As MessageDigest
			Try
				md = MessageDigest.getInstance("MD5")
			Catch t As Exception
				Throw New Exception(t)
			End Try
			For i As Integer = 0 To opInputs.Length - 1
				If opInputs(i).Empty Then
					Continue For
				End If

				'Need to hash - to ensure zero changes to input array
				Dim before() As SByte = opInputs(i).data().asBytes()
				Dim after As INDArray = Me.opInputsOrig(i)
				Dim dealloc As Boolean = False
				If opInputs(i).ordering() <> opInputsOrig(i).ordering() OrElse opInputs(i).stride().SequenceEqual(opInputsOrig(i).stride()) OrElse opInputs(i).elementWiseStride() <> opInputsOrig(i).elementWiseStride() Then
					'Clone if required (otherwise fails for views etc)
					after = opInputsOrig(i).dup()
					dealloc = True
				End If
				Dim afterB() As SByte = after.data().asBytes()
				Dim hash1() As SByte = md.digest(before)
				Dim hash2() As SByte = md.digest(afterB)

				Dim eq As Boolean = hash1.SequenceEqual(hash2)
				If eq Then
					passCounter.addAndGet(1)
				Else
					failCounter.addAndGet(1)
				End If

				Preconditions.checkState(eq, "Input array for non-inplace op was modified during execution " & "for op %s - input %s", op.Op.GetType(), i)

				'Deallocate:
				If dealloc AndAlso after.closeable() Then
					after.close()
				End If
				If opInputs(i).closeable() Then
					opInputs(i).close()
				End If
			Next i
		End Sub

		Public Overrides Function isActive(ByVal operation As Operation) As Boolean
			Return True
		End Function
	End Class

End Namespace