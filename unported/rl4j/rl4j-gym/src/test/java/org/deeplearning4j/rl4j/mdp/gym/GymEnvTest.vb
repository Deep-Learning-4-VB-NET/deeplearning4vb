Imports org.deeplearning4j.gym
Imports org.deeplearning4j.rl4j.space
Imports Box = org.deeplearning4j.rl4j.space.Box
Imports DiscreteSpace = org.deeplearning4j.rl4j.space.DiscreteSpace
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertArrayEquals
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertNotEquals

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

Namespace org.deeplearning4j.rl4j.mdp.gym

	''' 
	''' <summary>
	''' @author saudet
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @NativeTag public class GymEnvTest
	Public Class GymEnvTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled("Permissions issues on CI") public void testCartpole()
		Public Overridable Sub testCartpole()
			Dim mdp As New GymEnv("CartPole-v0", False, False)
			assertArrayEquals(New Integer() {4}, mdp.getObservationSpace().Shape)
			assertEquals(2, mdp.getActionSpace().getSize())
			assertEquals(False, mdp.isDone())
			Dim o As Box = DirectCast(mdp.reset(), Box)
			Dim r As StepReply = mdp.step(0)
			assertEquals(4, o.Data.shape()(0))
			assertEquals(4, CType(r.getObservation(), Box).Data.shape()(0))
			assertNotEquals(Nothing, mdp.newInstance())
			mdp.close()
		End Sub
	End Class

End Namespace