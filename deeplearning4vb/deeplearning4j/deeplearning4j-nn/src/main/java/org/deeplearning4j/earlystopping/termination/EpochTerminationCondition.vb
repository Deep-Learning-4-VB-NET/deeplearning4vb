Imports JsonInclude = org.nd4j.shade.jackson.annotation.JsonInclude
Imports JsonSubTypes = org.nd4j.shade.jackson.annotation.JsonSubTypes
Imports JsonTypeInfo = org.nd4j.shade.jackson.annotation.JsonTypeInfo

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

Namespace org.deeplearning4j.earlystopping.termination



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @JsonTypeInfo(use = JsonTypeInfo.Id.@CLASS, include = JsonTypeInfo.@As.@PROPERTY, property = "@class") @JsonInclude(JsonInclude.Include.NON_NULL) public interface EpochTerminationCondition extends java.io.Serializable
	Public Interface EpochTerminationCondition

		''' <summary>
		''' Initialize the epoch termination condition (often a no-op) </summary>
		Sub initialize()

		''' <summary>
		'''Should the early stopping training terminate at this epoch, based on the calculated score and the epoch number?
		''' Returns true if training should terminated, or false otherwise </summary>
		''' <param name="epochNum"> Number of the last completed epoch (starting at 0) </param>
		''' <param name="score"> Score calculate for this epoch </param>
		''' <returns> Whether training should be terminated at this epoch </returns>
		Function terminate(ByVal epochNum As Integer, ByVal score As Double, ByVal minimize As Boolean) As Boolean

	End Interface

End Namespace