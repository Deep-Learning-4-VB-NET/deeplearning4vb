Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports CustomOp = org.nd4j.linalg.api.ops.CustomOp
Imports Op = org.nd4j.linalg.api.ops.Op
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MatchCondition = org.nd4j.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports ND4JOpProfilerException = org.nd4j.linalg.exception.ND4JOpProfilerException
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Conditions = org.nd4j.linalg.indexing.conditions.Conditions
Imports OpProfiler = org.nd4j.linalg.profiler.OpProfiler

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

Namespace org.nd4j.linalg.api.ops.executioner


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class OpExecutionerUtil
	Public Class OpExecutionerUtil

		Private Sub New()
		End Sub

		Public Shared Sub checkForNaN(ByVal z As INDArray)
			If Not OpProfiler.Instance.getConfig().isCheckForNAN() Then
				Return
			End If

			If z.Empty OrElse Not z.dataType().isFPType() Then
				Return
			End If

			Dim match As Integer = 0
			If Not z.Scalar Then
				Dim condition As New MatchCondition(z, Conditions.Nan)
				match = Nd4j.Executioner.exec(condition).getInt(0)
			Else
				If z.data().dataType() = DataType.DOUBLE Then
					If Double.IsNaN(z.getDouble(0)) Then
						match = 1
					End If
				Else
					If Single.IsNaN(z.getFloat(0)) Then
						match = 1
					End If
				End If
			End If

			If match > 0 Then
				Throw New ND4JOpProfilerException("P.A.N.I.C.! Op.Z() contains " & match & " NaN value(s)")
			End If
		End Sub

		Public Shared Sub checkForAny(ByVal z As INDArray)
			checkForNaN(z)
			checkForInf(z)
		End Sub

		Public Shared Sub checkForInf(ByVal z As INDArray)
			If Not OpProfiler.Instance.getConfig().isCheckForINF() Then
				Return
			End If

			If z.Empty OrElse Not z.dataType().isFPType() Then
				Return
			End If

			Dim match As Integer = 0
			If Not z.Scalar Then
				Dim condition As New MatchCondition(z, Conditions.Infinite)
				match = Nd4j.Executioner.exec(condition).getInt(0)
			Else
				If z.data().dataType() = DataType.DOUBLE Then
					If Double.IsInfinity(z.getDouble(0)) Then
						match = 1
					End If
				Else
					If Single.IsInfinity(z.getFloat(0)) Then
						match = 1
					End If
				End If
			End If

			If match > 0 Then
				Throw New ND4JOpProfilerException("P.A.N.I.C.! Op.Z() contains " & match & " Inf value(s)")
			End If

		End Sub

		Public Shared Sub checkForNaN(ByVal op As Op, ByVal oc As OpContext)
			If Not OpProfiler.Instance.getConfig().isCheckForNAN() Then
				Return
			End If

			Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), op.z())
			If z IsNot Nothing AndAlso Not (TypeOf op Is MatchCondition) Then
				checkForNaN(z)
			End If
		End Sub

		Public Shared Sub checkForInf(ByVal op As Op, ByVal oc As OpContext)
			If Not OpProfiler.Instance.getConfig().isCheckForINF() Then
				Return
			End If

			Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), op.z())
			If z IsNot Nothing AndAlso Not (TypeOf op Is MatchCondition) Then
				checkForInf(z)
			End If
		End Sub

		Public Shared Sub checkForInf(ByVal op As CustomOp, ByVal oc As OpContext)
			If Not OpProfiler.Instance.getConfig().isCheckForINF() Then
				Return
			End If

			Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
			Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

			For Each input As val In inArgs
				checkForInf(input)
			Next input

			For Each output As val In outArgs
				checkForInf(output)
			Next output
		End Sub


		Public Shared Sub checkForNaN(ByVal op As CustomOp, ByVal oc As OpContext)
			If Not OpProfiler.Instance.getConfig().isCheckForNAN() Then
				Return
			End If

			Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
			Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

			For Each input As val In inArgs
				checkForNaN(input)
			Next input

			For Each output As val In outArgs
				checkForNaN(output)
			Next output
		End Sub
	End Class

End Namespace