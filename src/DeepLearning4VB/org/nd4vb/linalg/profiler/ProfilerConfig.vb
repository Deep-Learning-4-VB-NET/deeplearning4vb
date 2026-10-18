Imports System
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

Namespace org.nd4vb.linalg.profiler

    ''' <summary>
    ''' The op profiler switches (Java <c>ProfilerConfig</c>). Lombok's <c>@Data @Builder</c> members are written out,
    ''' with the <c>@Builder.Default</c> values as the defaults of the builder and of the field initializers:
    ''' <c>ProfilerConfig.builder().checkForNAN(True).build()</c>. The getters are Lombok's <c>isX()</c> methods.
    ''' </summary>
    Public Class ProfilerConfig

        Private notOptimalArguments As Boolean = False
        Private notOptimalTAD As Boolean = False
        Private nativeStatistics As Boolean = False
        Private checkForNAN As Boolean = False
        Private checkForINF As Boolean = False
        Private stackTrace As Boolean = False
        Private checkElapsedTime As Boolean = False
        ''' <summary>If enabled, each pointer will be workspace validation will be performed one each call</summary>
        Private checkWorkspaces As Boolean = True
        ''' <summary>If enabled, thread&lt;-&gt;device affinity will be checked on each call. PLEASE NOTE: everything will gets slower</summary>
        Private checkLocality As Boolean = False

        ''' <summary>Lombok's all-arguments constructor (used by the builder).</summary>
        Public Sub New(ByVal notOptimalArguments As Boolean, ByVal notOptimalTAD As Boolean, ByVal nativeStatistics As Boolean, ByVal checkForNAN As Boolean, ByVal checkForINF As Boolean, ByVal stackTrace As Boolean, ByVal checkElapsedTime As Boolean, ByVal checkWorkspaces As Boolean, ByVal checkLocality As Boolean)
            Me.notOptimalArguments = notOptimalArguments
            Me.notOptimalTAD = notOptimalTAD
            Me.nativeStatistics = nativeStatistics
            Me.checkForNAN = checkForNAN
            Me.checkForINF = checkForINF
            Me.stackTrace = stackTrace
            Me.checkElapsedTime = checkElapsedTime
            Me.checkWorkspaces = checkWorkspaces
            Me.checkLocality = checkLocality
        End Sub

        Public Shared Function builder() As ProfilerConfigBuilder
            Return New ProfilerConfigBuilder()
        End Function

        Public Overridable Function isNotOptimalArguments() As Boolean
            Return notOptimalArguments
        End Function

        Public Overridable Sub setNotOptimalArguments(ByVal notOptimalArguments As Boolean)
            Me.notOptimalArguments = notOptimalArguments
        End Sub

        Public Overridable Function isNotOptimalTAD() As Boolean
            Return notOptimalTAD
        End Function

        Public Overridable Sub setNotOptimalTAD(ByVal notOptimalTAD As Boolean)
            Me.notOptimalTAD = notOptimalTAD
        End Sub

        Public Overridable Function isNativeStatistics() As Boolean
            Return nativeStatistics
        End Function

        Public Overridable Sub setNativeStatistics(ByVal nativeStatistics As Boolean)
            Me.nativeStatistics = nativeStatistics
        End Sub

        Public Overridable Function isCheckForNAN() As Boolean
            Return checkForNAN
        End Function

        Public Overridable Sub setCheckForNAN(ByVal checkForNAN As Boolean)
            Me.checkForNAN = checkForNAN
        End Sub

        Public Overridable Function isCheckForINF() As Boolean
            Return checkForINF
        End Function

        Public Overridable Sub setCheckForINF(ByVal checkForINF As Boolean)
            Me.checkForINF = checkForINF
        End Sub

        Public Overridable Function isStackTrace() As Boolean
            Return stackTrace
        End Function

        Public Overridable Sub setStackTrace(ByVal stackTrace As Boolean)
            Me.stackTrace = stackTrace
        End Sub

        Public Overridable Function isCheckElapsedTime() As Boolean
            Return checkElapsedTime
        End Function

        Public Overridable Sub setCheckElapsedTime(ByVal checkElapsedTime As Boolean)
            Me.checkElapsedTime = checkElapsedTime
        End Sub

        Public Overridable Function isCheckWorkspaces() As Boolean
            Return checkWorkspaces
        End Function

        Public Overridable Sub setCheckWorkspaces(ByVal checkWorkspaces As Boolean)
            Me.checkWorkspaces = checkWorkspaces
        End Sub

        Public Overridable Function isCheckLocality() As Boolean
            Return checkLocality
        End Function

        Public Overridable Sub setCheckLocality(ByVal checkLocality As Boolean)
            Me.checkLocality = checkLocality
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True
            Dim o As ProfilerConfig = TryCast(obj, ProfilerConfig)
            If o Is Nothing OrElse CObj(o).GetType() IsNot CObj(Me).GetType() Then Return False
            Return notOptimalArguments.Equals(o.notOptimalArguments) AndAlso notOptimalTAD.Equals(o.notOptimalTAD) AndAlso nativeStatistics.Equals(o.nativeStatistics) AndAlso checkForNAN.Equals(o.checkForNAN) AndAlso checkForINF.Equals(o.checkForINF) AndAlso stackTrace.Equals(o.stackTrace) AndAlso checkElapsedTime.Equals(o.checkElapsedTime) AndAlso checkWorkspaces.Equals(o.checkWorkspaces) AndAlso checkLocality.Equals(o.checkLocality)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h As Integer = 1
            h = h * 59 + notOptimalArguments.GetHashCode()
            h = h * 59 + notOptimalTAD.GetHashCode()
            h = h * 59 + nativeStatistics.GetHashCode()
            h = h * 59 + checkForNAN.GetHashCode()
            h = h * 59 + checkForINF.GetHashCode()
            h = h * 59 + stackTrace.GetHashCode()
            h = h * 59 + checkElapsedTime.GetHashCode()
            h = h * 59 + checkWorkspaces.GetHashCode()
            h = h * 59 + checkLocality.GetHashCode()
            Return h
        End Function

        Public Overrides Function ToString() As String
            Return "ProfilerConfig(" & "notOptimalArguments=" & If(notOptimalArguments, "true", "false") & ", notOptimalTAD=" & If(notOptimalTAD, "true", "false") & ", nativeStatistics=" & If(nativeStatistics, "true", "false") & ", checkForNAN=" & If(checkForNAN, "true", "false") & ", checkForINF=" & If(checkForINF, "true", "false") & ", stackTrace=" & If(stackTrace, "true", "false") & ", checkElapsedTime=" & If(checkElapsedTime, "true", "false") & ", checkWorkspaces=" & If(checkWorkspaces, "true", "false") & ", checkLocality=" & If(checkLocality, "true", "false") & ")"
        End Function

        ''' <summary>Lombok's builder for <see cref="ProfilerConfig"/>.</summary>
        Public Class ProfilerConfigBuilder

            Private notOptimalArgumentsValue As Boolean = False
            Private notOptimalTADValue As Boolean = False
            Private nativeStatisticsValue As Boolean = False
            Private checkForNANValue As Boolean = False
            Private checkForINFValue As Boolean = False
            Private stackTraceValue As Boolean = False
            Private checkElapsedTimeValue As Boolean = False
            Private checkWorkspacesValue As Boolean = True
            Private checkLocalityValue As Boolean = False

            Public Function notOptimalArguments(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.notOptimalArgumentsValue = value
                Return Me
            End Function

            Public Function notOptimalTAD(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.notOptimalTADValue = value
                Return Me
            End Function

            Public Function nativeStatistics(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.nativeStatisticsValue = value
                Return Me
            End Function

            Public Function checkForNAN(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.checkForNANValue = value
                Return Me
            End Function

            Public Function checkForINF(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.checkForINFValue = value
                Return Me
            End Function

            Public Function stackTrace(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.stackTraceValue = value
                Return Me
            End Function

            Public Function checkElapsedTime(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.checkElapsedTimeValue = value
                Return Me
            End Function

            Public Function checkWorkspaces(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.checkWorkspacesValue = value
                Return Me
            End Function

            Public Function checkLocality(ByVal value As Boolean) As ProfilerConfigBuilder
                Me.checkLocalityValue = value
                Return Me
            End Function

            Public Function build() As ProfilerConfig
                Return New ProfilerConfig(notOptimalArgumentsValue, notOptimalTADValue, nativeStatisticsValue, checkForNANValue, checkForINFValue, stackTraceValue, checkElapsedTimeValue, checkWorkspacesValue, checkLocalityValue)
            End Function

        End Class

    End Class

End Namespace
