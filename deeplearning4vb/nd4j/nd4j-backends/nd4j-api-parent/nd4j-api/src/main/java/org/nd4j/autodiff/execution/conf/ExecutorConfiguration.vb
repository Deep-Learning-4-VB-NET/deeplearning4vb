Imports FlatBufferBuilder = com.google.flatbuffers.FlatBufferBuilder
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Direction = org.nd4j.graph.Direction
Imports FlatConfiguration = org.nd4j.graph.FlatConfiguration
Imports ProfilingMode = org.nd4j.graph.ProfilingMode
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner

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

Namespace org.nd4j.autodiff.execution.conf

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @Slf4j @NoArgsConstructor @AllArgsConstructor @Builder public class ExecutorConfiguration
	Public Class ExecutorConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode profilingMode = org.nd4j.linalg.api.ops.executioner.OpExecutioner.ProfilingMode.DISABLED;
		Private profilingMode As OpExecutioner.ProfilingMode = OpExecutioner.ProfilingMode.DISABLED
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private ExecutionMode executionMode = ExecutionMode.SEQUENTIAL;
		Private executionMode As ExecutionMode = ExecutionMode.SEQUENTIAL
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private OutputMode outputMode = OutputMode.IMPLICIT;
		Private outputMode As OutputMode = OutputMode.IMPLICIT
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default boolean gatherTimings = true;
		Friend gatherTimings As Boolean = True
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long footprintForward = 0L;
		Private footprintForward As Long = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default private long footprintBackward = 0L;
		Private footprintBackward As Long = 0L


		''' <summary>
		''' This method </summary>
		''' <param name="builder">
		''' @return </param>
		Public Overridable Function getFlatConfiguration(ByVal builder As FlatBufferBuilder) As Integer

			Dim prof As SByte = If(profilingMode = OpExecutioner.ProfilingMode.INF_PANIC, ProfilingMode.INF_PANIC, If(profilingMode = OpExecutioner.ProfilingMode.NAN_PANIC, ProfilingMode.NAN_PANIC, If(profilingMode = OpExecutioner.ProfilingMode.ANY_PANIC, ProfilingMode.ANY_PANIC, ProfilingMode.NONE)))

			Dim exec As SByte = If(executionMode = ExecutionMode.SEQUENTIAL, org.nd4j.graph.ExecutionMode.SEQUENTIAL, If(executionMode = ExecutionMode.AUTO, org.nd4j.graph.ExecutionMode.AUTO, If(executionMode = ExecutionMode.STRICT, org.nd4j.graph.ExecutionMode.STRICT, -1)))

			Dim outp As SByte = If(outputMode = OutputMode.IMPLICIT, org.nd4j.graph.OutputMode.IMPLICIT, If(outputMode = OutputMode.EXPLICIT, org.nd4j.graph.OutputMode.EXPLICIT, If(outputMode = OutputMode.EXPLICIT_AND_IMPLICIT, org.nd4j.graph.OutputMode.EXPLICIT_AND_IMPLICIT, If(outputMode = OutputMode.VARIABLE_SPACE, org.nd4j.graph.OutputMode.VARIABLE_SPACE, -1))))

			If exec = -1 Then
				Throw New System.NotSupportedException("Unknown values were passed into configuration as ExecutionMode: [" & executionMode & "]")
			End If

			If outp = -1 Then
				Throw New System.NotSupportedException("Unknown values were passed into configuration as OutputMode: [" & outputMode & "]")
			End If

			Return FlatConfiguration.createFlatConfiguration(builder, -1, prof, exec, outp, gatherTimings, footprintForward, footprintBackward, Direction.FORWARD_ONLY)
		End Function
	End Class

End Namespace