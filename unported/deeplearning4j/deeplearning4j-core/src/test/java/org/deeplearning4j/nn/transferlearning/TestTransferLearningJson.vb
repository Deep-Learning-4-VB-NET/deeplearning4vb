Imports BaseDL4JTest = org.deeplearning4j.BaseDL4JTest
Imports Test = org.junit.jupiter.api.Test
Imports Activation = org.nd4j.linalg.activations.Activation
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
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

Namespace org.deeplearning4j.nn.transferlearning

	Public Class TestTransferLearningJson
		Inherits BaseDL4JTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testJsonYaml()
		Public Overridable Sub testJsonYaml()

			Dim c As FineTuneConfiguration = (New FineTuneConfiguration.Builder()).activation(Activation.ELU).updater(New AdaGrad(1.0)).biasUpdater(New AdaGrad(10.0)).build()

			Dim asJson As String = c.toJson()
			Dim asYaml As String = c.toYaml()

			Dim fromJson As FineTuneConfiguration = FineTuneConfiguration.fromJson(asJson)
			Dim fromYaml As FineTuneConfiguration = FineTuneConfiguration.fromYaml(asYaml)

			'        System.out.println(asJson);

			assertEquals(c, fromJson)
			assertEquals(c, fromYaml)
			assertEquals(asJson, fromJson.toJson())
			assertEquals(asYaml, fromYaml.toYaml())
		End Sub

	End Class

End Namespace