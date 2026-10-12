Imports val = lombok.val
Imports Test = org.junit.jupiter.api.Test
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports WorkspaceConfiguration = org.nd4j.linalg.api.memory.conf.WorkspaceConfiguration
Imports AllocationPolicy = org.nd4j.linalg.api.memory.enums.AllocationPolicy
Imports LearningPolicy = org.nd4j.linalg.api.memory.enums.LearningPolicy
Imports ResetPolicy = org.nd4j.linalg.api.memory.enums.ResetPolicy
Imports SpillPolicy = org.nd4j.linalg.api.memory.enums.SpillPolicy
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports org.junit.jupiter.api.Assertions

' ******************************************************************************
' *
' *
' * This program and the accompanying materials are made available under the
' * terms of the Apache License, Version 2.0 which is available at
' * https://www.apache.org/licenses/LICENSE-2.0.
' *
' *  See the NOTICE file distributed with this work for additional
' *  information regarding copyright ownership.
' * Unless required by applicable law or agreed to in writing, software
' * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' * License for the specific language governing permissions and limitations
' * under the License.
' *
' * SPDX-License-Identifier: Apache-2.0
' *****************************************************************************

Namespace org.nd4j.jita.workspace

	Public Class CudaWorkspaceTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCircularWorkspaceAsymmetry_1()
		Public Overridable Sub testCircularWorkspaceAsymmetry_1()
			' circular workspace mode
			Dim configuration As val = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.STRICT).policySpill(SpillPolicy.FAIL).policyLearning(LearningPolicy.NONE).build()


			Using ws As lombok.val = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "circular_ws"), CudaWorkspace)
				Dim array As val = Nd4j.create(DataType.FLOAT, 10, 10)

				assertEquals(0, ws.getHostOffset())
				assertNotEquals(0, ws.getDeviceOffset())

				' we expect that this array has no data/buffer on HOST side
				assertEquals(AffinityManager.Location.DEVICE, Nd4j.AffinityManager.getActiveLocation(array))

				' since this array doesn't have HOST buffer - it will allocate one now
				array.getDouble(3L)

				assertEquals(ws.getHostOffset(), ws.getDeviceOffset())
			End Using

			Using ws As lombok.val = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "circular_ws"), CudaWorkspace)
				assertEquals(ws.getHostOffset(), ws.getDeviceOffset())
			End Using

			Nd4j.WorkspaceManager.destroyAllWorkspacesForCurrentThread()
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCircularWorkspaceAsymmetry_2()
		Public Overridable Sub testCircularWorkspaceAsymmetry_2()
			' circular workspace mode
			Dim configuration As val = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.STRICT).policySpill(SpillPolicy.FAIL).policyLearning(LearningPolicy.NONE).build()

			Dim root As val = Nd4j.create(DataType.FLOAT, 1000000).assign(119)

			For e As Integer = 0 To 99
				Using ws As lombok.val = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "circular_ws"), CudaWorkspace)
					Dim array As val = Nd4j.create(DataType.FLOAT, root.shape())
					array.assign(root)

					array.data().getInt(3)

					assertEquals(ws.getHostOffset(), ws.getDeviceOffset())
				End Using
			Next e
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCircularWorkspaceAsymmetry_3()
		Public Overridable Sub testCircularWorkspaceAsymmetry_3()
			' circular workspace mode
			Dim configuration As val = WorkspaceConfiguration.builder().initialSize(10 * 1024 * 1024).policyReset(ResetPolicy.ENDOFBUFFER_REACHED).policyAllocation(AllocationPolicy.STRICT).policySpill(SpillPolicy.FAIL).policyLearning(LearningPolicy.NONE).build()

			Dim root As val = Nd4j.create(DataType.FLOAT, 1000000).assign(119)

			For e As Integer = 0 To 99
				Using ws As lombok.val = DirectCast(org.nd4j.linalg.factory.Nd4j.WorkspaceManager.getAndActivateWorkspace(configuration, "circular_ws"), CudaWorkspace)
					Dim array As val = Nd4j.create(DataType.FLOAT, root.shape())
					array.assign(root)

					Dim second As val = Nd4j.create(DataType.FLOAT, root.shape())

					array.data().getInt(3)
				End Using
			Next e
		End Sub
	End Class
End Namespace