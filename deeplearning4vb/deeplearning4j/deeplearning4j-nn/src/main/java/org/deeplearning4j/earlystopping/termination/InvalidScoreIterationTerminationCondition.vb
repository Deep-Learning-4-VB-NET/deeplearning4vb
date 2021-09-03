Imports System
Imports Data = lombok.Data

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
'ORIGINAL LINE: @Data public class InvalidScoreIterationTerminationCondition implements IterationTerminationCondition
	<Serializable>
	Public Class InvalidScoreIterationTerminationCondition
		Implements IterationTerminationCondition

		Public Overridable Sub initialize() Implements IterationTerminationCondition.initialize
			'No op
		End Sub

		Public Overridable Function terminate(ByVal lastMiniBatchScore As Double) As Boolean Implements IterationTerminationCondition.terminate
			Return Double.IsNaN(lastMiniBatchScore) OrElse Double.IsInfinity(lastMiniBatchScore)
		End Function

		Public Overrides Function ToString() As String
			Return "InvalidScoreIterationTerminationCondition()"
		End Function
	End Class

End Namespace