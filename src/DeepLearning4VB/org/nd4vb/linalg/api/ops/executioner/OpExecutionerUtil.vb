Imports System
Imports System.Collections.Generic
Imports DeepLearning4VB.org.nd4vb.linalg.api.buffer
Imports INDArray = DeepLearning4VB.org.nd4vb.linalg.api.ndarray.INDArray
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports MatchCondition = DeepLearning4VB.org.nd4vb.linalg.api.ops.impl.reduce.longer.MatchCondition
Imports ND4VBOpProfilerException = DeepLearning4VB.org.nd4vb.linalg.exception.ND4VBOpProfilerException
Imports Nd4vbT = DeepLearning4VB.org.nd4vb.linalg.factory.Nd4vb
Imports Conditions = DeepLearning4VB.org.nd4vb.linalg.indexing.conditions.Conditions
Imports OpProfiler = DeepLearning4VB.org.nd4vb.linalg.profiler.OpProfiler
Imports ProfilerConfig = DeepLearning4VB.org.nd4vb.linalg.profiler.ProfilerConfig

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

Namespace org.nd4vb.linalg.api.ops.executioner

    ''' <summary>
    ''' The NaN and infinity panics of the op profiler (Java <c>OpExecutionerUtil</c>): when the
    ''' <see cref="OpProfiler"/> configuration asks for them, op outputs (and custom op inputs) are checked and an
    ''' <see cref="ND4VBOpProfilerException"/> is thrown when they contain NaN or infinite values. Non-scalar arrays are
    ''' counted with a <see cref="MatchCondition"/> op run by <c>Nd4vb.Executioner</c>.
    ''' <para>
    ''' Difference from Java: a missing profiler configuration (Nothing) means "no checks" (Java threw a
    ''' <c>NullPointerException</c>).
    ''' </para>
    ''' </summary>
    Public Class OpExecutionerUtil

        Private Sub New()
        End Sub

        Private Shared ReadOnly Property checkNaN As Boolean
            Get
                Dim config As ProfilerConfig = OpProfiler.Instance.Config
                Return config IsNot Nothing AndAlso config.isCheckForNAN()
            End Get
        End Property

        Private Shared ReadOnly Property checkInf As Boolean
            Get
                Dim config As ProfilerConfig = OpProfiler.Instance.Config
                Return config IsNot Nothing AndAlso config.isCheckForINF()
            End Get
        End Property

        ''' <summary>Throws when NaN checks are on and the floating-point array contains NaN values.</summary>
        ''' <exception cref="ND4VBOpProfilerException">The array contains NaN.</exception>
        Public Shared Sub checkForNaN(ByVal z As INDArray)
            If Not checkNaN Then
                Return
            End If

            If z.Empty OrElse Not z.dataType().isFPType() Then
                Return
            End If

            Dim match As Integer = 0
            If Not z.Scalar Then
                Dim condition As New MatchCondition(z, Conditions.Nan)
                match = Nd4vbT.Executioner.exec(condition).getInt(0)
            Else
                If z.data().dataType() = DataType.DOUBLE Then
                    If Double.IsNaN(z.getDouble(0L)) Then
                        match = 1
                    End If
                Else
                    If Single.IsNaN(z.getFloat(0L)) Then
                        match = 1
                    End If
                End If
            End If

            If match > 0 Then
                Throw New ND4VBOpProfilerException("P.A.N.I.C.! Op.Z() contains " & match & " NaN value(s)")
            End If
        End Sub

        ''' <summary>Runs <see cref="checkForNaN(INDArray)"/> and <see cref="checkForInf(INDArray)"/>.</summary>
        Public Shared Sub checkForAny(ByVal z As INDArray)
            checkForNaN(z)
            checkForInf(z)
        End Sub

        ''' <summary>Throws when infinity checks are on and the floating-point array contains infinite values.</summary>
        ''' <exception cref="ND4VBOpProfilerException">The array contains an infinity.</exception>
        Public Shared Sub checkForInf(ByVal z As INDArray)
            If Not checkInf Then
                Return
            End If

            If z.Empty OrElse Not z.dataType().isFPType() Then
                Return
            End If

            Dim match As Integer = 0
            If Not z.Scalar Then
                Dim condition As New MatchCondition(z, Conditions.Infinite)
                match = Nd4vbT.Executioner.exec(condition).getInt(0)
            Else
                If z.data().dataType() = DataType.DOUBLE Then
                    If Double.IsInfinity(z.getDouble(0L)) Then
                        match = 1
                    End If
                Else
                    If Single.IsInfinity(z.getFloat(0L)) Then
                        match = 1
                    End If
                End If
            End If

            If match > 0 Then
                Throw New ND4VBOpProfilerException("P.A.N.I.C.! Op.Z() contains " & match & " Inf value(s)")
            End If

        End Sub

        ''' <summary>Checks z of the op (output 0 of the context when one is given) for NaN values.</summary>
        Public Shared Sub checkForNaN(ByVal op As Op, ByVal oc As OpContext)
            If Not checkNaN Then
                Return
            End If

            Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), op.z())
            If z IsNot Nothing AndAlso Not (TypeOf op Is MatchCondition) Then
                checkForNaN(z)
            End If
        End Sub

        ''' <summary>Checks z of the op (output 0 of the context when one is given) for infinite values.</summary>
        Public Shared Sub checkForInf(ByVal op As Op, ByVal oc As OpContext)
            If Not checkInf Then
                Return
            End If

            Dim z As INDArray = If(oc IsNot Nothing, oc.getOutputArray(0), op.z())
            If z IsNot Nothing AndAlso Not (TypeOf op Is MatchCondition) Then
                checkForInf(z)
            End If
        End Sub

        ''' <summary>Checks the inputs and outputs of the custom op (from the context when one is given) for infinite values.</summary>
        Public Shared Sub checkForInf(ByVal op As CustomOp, ByVal oc As OpContext)
            If Not checkInf Then
                Return
            End If

            Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
            Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

            For Each input As INDArray In inArgs
                checkForInf(input)
            Next input

            For Each output As INDArray In outArgs
                checkForInf(output)
            Next output
        End Sub

        ''' <summary>Checks the inputs and outputs of the custom op (from the context when one is given) for NaN values.</summary>
        Public Shared Sub checkForNaN(ByVal op As CustomOp, ByVal oc As OpContext)
            If Not checkNaN Then
                Return
            End If

            Dim inArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getInputArrays(), op.inputArguments())
            Dim outArgs As IList(Of INDArray) = If(oc IsNot Nothing, oc.getOutputArrays(), op.outputArguments())

            For Each input As INDArray In inArgs
                checkForNaN(input)
            Next input

            For Each output As INDArray In outArgs
                checkForNaN(output)
            Next output
        End Sub
    End Class

End Namespace
