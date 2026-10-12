Imports System
Imports AllArgsConstructor = lombok.AllArgsConstructor
Imports Builder = lombok.Builder
Imports Data = lombok.Data
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports org.nd4j.linalg.api.memory.enums

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

Namespace org.nd4j.linalg.api.memory.conf


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder @Data @NoArgsConstructor @AllArgsConstructor public class WorkspaceConfiguration implements java.io.Serializable
	<Serializable>
	Public Class WorkspaceConfiguration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected AllocationPolicy policyAllocation = AllocationPolicy.OVERALLOCATE;
		Protected Friend policyAllocation As AllocationPolicy = AllocationPolicy.OVERALLOCATE
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected SpillPolicy policySpill = SpillPolicy.EXTERNAL;
		Protected Friend policySpill As SpillPolicy = SpillPolicy.EXTERNAL
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected MirroringPolicy policyMirroring = MirroringPolicy.FULL;
		Protected Friend policyMirroring As MirroringPolicy = MirroringPolicy.FULL
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected LearningPolicy policyLearning = LearningPolicy.FIRST_LOOP;
		Protected Friend policyLearning As LearningPolicy = LearningPolicy.FIRST_LOOP
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected ResetPolicy policyReset = ResetPolicy.BLOCK_LEFT;
		Protected Friend policyReset As ResetPolicy = ResetPolicy.BLOCK_LEFT
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected LocationPolicy policyLocation = LocationPolicy.RAM;
		Protected Friend policyLocation As LocationPolicy = LocationPolicy.RAM

		''' <summary>
		''' Path to file to be memory-mapped
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected String tempFilePath = null;
		Protected Friend tempFilePath As String = Nothing

		''' <summary>
		''' This variable specifies amount of memory allocated for this workspace during initialization
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long initialSize = 0;
		Protected Friend initialSize As Long = 0

		''' <summary>
		''' This variable specifies minimal workspace size
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long minSize = 0;
		Protected Friend minSize As Long = 0

		''' <summary>
		''' This variable specifies maximal workspace size
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long maxSize = 0;
		Protected Friend maxSize As Long = 0

		''' <summary>
		''' For workspaces with learnable size, this variable defines how many cycles will be spent during learning phase
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected int cyclesBeforeInitialization = 0;
		Protected Friend cyclesBeforeInitialization As Integer = 0

		''' <summary>
		''' If OVERALLOCATION policy is set, memory will be overallocated in addition to initialSize of learned size
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected double overallocationLimit = 0.3;
		Protected Friend overallocationLimit As Double = 0.3

		''' <summary>
		''' This value is used only for circular workspaces
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected int stepsNumber = 2;
		Protected Friend stepsNumber As Integer = 2
	End Class

End Namespace