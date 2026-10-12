Imports System
Imports lombok
Imports WorkspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode
Imports MessageHandler = org.deeplearning4j.optimize.solvers.accumulation.MessageHandler
Imports ResidualPostProcessor = org.deeplearning4j.optimize.solvers.accumulation.encoding.ResidualPostProcessor
Imports ThresholdAlgorithm = org.deeplearning4j.optimize.solvers.accumulation.encoding.ThresholdAlgorithm
Imports VoidConfiguration = org.nd4j.parameterserver.distributed.conf.VoidConfiguration

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

Namespace org.deeplearning4j.spark.parameterserver.conf


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data @NoArgsConstructor @AllArgsConstructor @Builder public class SharedTrainingConfiguration implements java.io.Serializable
	<Serializable>
	Public Class SharedTrainingConfiguration
		Protected Friend voidConfiguration As VoidConfiguration

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected org.deeplearning4j.nn.conf.WorkspaceMode workspaceMode = org.deeplearning4j.nn.conf.WorkspaceMode.ENABLED;
		Protected Friend workspaceMode As WorkspaceMode = WorkspaceMode.ENABLED
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected int prefetchSize = 2;
		Protected Friend prefetchSize As Integer = 2
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected boolean epochReset = false;
		Protected Friend epochReset As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected int numberOfWorkersPerNode = -1;
		Protected Friend numberOfWorkersPerNode As Integer = -1
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected long debugLongerIterations = 0L;
		Protected Friend debugLongerIterations As Long = 0L
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected boolean encodingDebugMode = false;
		Protected Friend encodingDebugMode As Boolean = False

		''' <summary>
		''' This value **overrides** bufferSize calculations for gradients accumulator
		''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Builder.@Default protected int bufferSize = 0;
		Protected Friend bufferSize As Integer = 0

		Protected Friend thresholdAlgorithm As ThresholdAlgorithm
		Protected Friend residualPostProcessor As ResidualPostProcessor
'JAVA TO VB CONVERTER NOTE: The field messageHandlerClass was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend messageHandlerClass_Conflict As String



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setMessageHandlerClass(@NonNull String messageHandlerClass)
		Public Overridable WriteOnly Property MessageHandlerClass As String
			Set(ByVal messageHandlerClass As String)
				Me.messageHandlerClass_Conflict = messageHandlerClass
			End Set
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setMessageHandlerClass(@NonNull MessageHandler handler)
		Public Overridable WriteOnly Property MessageHandlerClass As MessageHandler
			Set(ByVal handler As MessageHandler)
	'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
				Me.messageHandlerClass_Conflict = handler.GetType().FullName
			End Set
		End Property
	End Class

End Namespace