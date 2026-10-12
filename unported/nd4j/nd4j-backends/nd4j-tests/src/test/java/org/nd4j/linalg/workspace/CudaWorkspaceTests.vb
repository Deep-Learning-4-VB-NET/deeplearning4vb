Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports MirroringPolicy = org.nd4j.linalg.api.memory.enums.MirroringPolicy
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutioner = org.nd4j.linalg.api.ops.executioner.OpExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.workspace

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.WORKSPACES) @NativeTag public class CudaWorkspaceTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CudaWorkspaceTests
		Inherits BaseNd4jTestWithBackends

		Private initialType As DataType = Nd4j.dataType()



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testWorkspaceReuse()
		Public Overridable Sub testWorkspaceReuse()
			If Nd4j.Executioner.type() <> OpExecutioner.ExecutionerType.CUDA Then
				Return
			End If

			Dim workspaceConfig As val = WorkspaceConfiguration.builder().policyMirroring(MirroringPolicy.HOST_ONLY).build()
			Dim cnt As Integer = 0

			For e As Integer = 0 To 9
				Using ws As lombok.val = org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(workspaceConfig, "test")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray zeros = org.nd4j.linalg.factory.Nd4j.zeros(4, "f"c);
					Dim zeros As INDArray = Nd4j.zeros(4, "f"c)
					'final INDArray zeros = Nd4j.create(4, 'f'); // Also fails, but maybe less of an issue as javadoc does not say that one can expect returned array to be all zeros.
					assertEquals(0R, zeros.sumNumber().doubleValue(), 1e-10,"Got non-zero array " & zeros & " after " & cnt & " iterations !")
					zeros.putScalar(0, 1)
				End Using
			Next e

		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace