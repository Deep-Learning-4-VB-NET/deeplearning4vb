Imports System
Imports val = lombok.val
Imports ND4JEnvironmentVars = org.nd4j.common.config.ND4JEnvironmentVars
Imports EnvironmentalAction = org.nd4j.linalg.env.EnvironmentalAction

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

Namespace org.nd4j.linalg.env.impl

	Public Class OmpNumThreadsAction
		Implements EnvironmentalAction

		Public Overridable Function targetVariable() As String Implements EnvironmentalAction.targetVariable
			Return ND4JEnvironmentVars.OMP_NUM_THREADS
		End Function

		Public Overridable Sub process(ByVal value As String) Implements EnvironmentalAction.process
			Dim v As val = Convert.ToInt32(value)

			Dim skipper As val = Environment.GetEnvironmentVariable(ND4JEnvironmentVars.ND4J_SKIP_BLAS_THREADS)
			If skipper Is Nothing Then
				' we infer num threads only if skipper undefined
				' Nd4j.setNumThreads(v);
				' method does not do anything anymore and was removed
			End If
		End Sub
	End Class

End Namespace