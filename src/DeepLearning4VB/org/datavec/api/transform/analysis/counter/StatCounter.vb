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

Imports System

Namespace org.datavec.api.transform.analysis.counter

    ''' <summary>
    ''' Running count, mean, variance (Welford), min and max of a stream of doubles; two counters can be merged
    ''' (Chan et al. parallel update). The minimum starts at <see cref="Double.MaxValue"/> and the maximum at
    ''' <c>-Double.MaxValue</c>, as in Java.
    ''' <para>Difference from Java: merging a non-empty counter into an empty one copies the other counter's
    ''' variance numerator. The Java code assigned it to the running mean by mistake
    ''' (<c>runningMean = o.runningM2</c>), which corrupted the mean and lost the variance.</para>
    ''' </summary>
    <Serializable>
    Public Class StatCounter

        Private countValue As Long = 0
        Private runningMean As Double
        Private runningM2 As Double ' Running variance numerator (sum of (x - mean)^2)
        Private maxValue As Double = -Double.MaxValue
        Private minValue As Double = Double.MaxValue

        Public Overridable ReadOnly Property Mean As Double
            Get
                Return runningMean
            End Get
        End Property

        Public Overridable ReadOnly Property Sum As Double
            Get
                Return runningMean * countValue
            End Get
        End Property

        Public Overridable ReadOnly Property Min As Double
            Get
                Return minValue
            End Get
        End Property

        Public Overridable ReadOnly Property Max As Double
            Get
                Return maxValue
            End Get
        End Property

        Public Overridable ReadOnly Property Count As Long
            Get
                Return countValue
            End Get
        End Property

        ''' <summary>
        ''' The population (divide by n) or sample (divide by n - 1) variance; NaN for an empty population or a
        ''' one-element sample.
        ''' </summary>
        Public Overridable Function getVariance(ByVal population As Boolean) As Double
            Dim divisor As Long = If(population, countValue, countValue - 1)
            If (population AndAlso countValue = 0) OrElse (Not population AndAlso countValue = 1) Then
                Return Double.NaN
            End If
            Return runningM2 / divisor
        End Function

        Public Overridable Function getStddev(ByVal population As Boolean) As Double
            Return Math.Sqrt(getVariance(population))
        End Function

        Public Overridable Sub add(ByVal x As Double)
            Dim d As Double = x - runningMean
            countValue += 1
            runningMean += (d / countValue)
            runningM2 += (d * (x - runningMean))
            maxValue = Math.Max(maxValue, x)
            minValue = Math.Min(minValue, x)
        End Sub

        ''' <summary>Merges the other counter into this one and returns this counter.</summary>
        Public Overridable Function merge(ByVal o As StatCounter) As StatCounter
            If o Is Nothing OrElse o.countValue = 0 Then
                Return Me
            End If
            If o Is Me Then
                Return merge(o.clone())
            End If
            If Me.countValue = 0 Then
                countValue = o.countValue
                runningMean = o.runningMean
                runningM2 = o.runningM2
                maxValue = o.maxValue
                minValue = o.minValue
            Else
                minValue = Math.Min(minValue, o.minValue)
                maxValue = Math.Max(maxValue, o.maxValue)

                Dim d As Double = o.runningMean - runningMean
                If o.countValue * 10 < countValue Then
                    runningMean = runningMean + (d * o.countValue) / (countValue + o.countValue)
                ElseIf countValue * 10 < o.countValue Then
                    runningMean = o.runningMean - (d * countValue) / (countValue + o.countValue)
                Else
                    runningMean = (runningMean * countValue + o.runningMean * o.countValue) / (countValue + o.countValue)
                End If
                runningM2 += o.runningM2 + (d * d * countValue * o.countValue) / (countValue + o.countValue)
                countValue += o.countValue
            End If

            Return Me
        End Function

        Public Overridable Function clone() As StatCounter
            Dim ret As New StatCounter()
            ret.countValue = countValue
            ret.runningMean = runningMean
            ret.runningM2 = runningM2
            ret.maxValue = maxValue
            ret.minValue = minValue
            Return ret
        End Function

        Public Overrides Function ToString() As String
            Return "StatCounter(count=" & countValue & ",mean=" & Arrays.JavaDoubleToString(runningMean) & ",stdev=" & Arrays.JavaDoubleToString(getStddev(False)) & ",min=" & Arrays.JavaDoubleToString(minValue) & ",max=" & Arrays.JavaDoubleToString(maxValue) & ")"
        End Function
    End Class

End Namespace
