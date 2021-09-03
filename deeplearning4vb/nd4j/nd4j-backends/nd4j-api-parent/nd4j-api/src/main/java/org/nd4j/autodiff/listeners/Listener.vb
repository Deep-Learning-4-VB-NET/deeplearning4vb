Imports LossCurve = org.nd4j.autodiff.listeners.records.LossCurve
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports Variable = org.nd4j.autodiff.samediff.internal.Variable
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpContext = org.nd4j.linalg.api.ops.OpContext
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet

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

Namespace org.nd4j.autodiff.listeners

	Public Interface Listener


		''' <summary>
		''' Required variables for this listener.
		''' <para>
		''' Used to ensure these variables end up in the minimum required subgraph calculated by <seealso cref="org.nd4j.autodiff.samediff.internal.InferenceSession"/>.
		''' Otherwise, if the variables weren't required by a loss variable, they would not be calculated.
		''' </para>
		''' <para>
		''' Any variables in here are guaranteed to have <seealso cref="Listener.activationAvailable(SameDiff, At, MultiDataSet, SameDiffOp, String, INDArray)"/>
		''' called for them, regardless of whether they would normally be calculated or not.
		''' </para>
		''' </summary>
		Function requiredVariables(ByVal sd As SameDiff) As ListenerVariables

		''' <summary>
		''' Returns whether this listener is active during the given operation. If this returns false for the given operation,
		''' those listener methods will not be called.
		''' </summary>
		Function isActive(ByVal operation As Operation) As Boolean

		''' <summary>
		''' Called at the start of every epoch, when fitting from an iterator
		''' </summary>
		''' <param name="sd"> The SameDiff instance </param>
		''' <param name="at"> Current iteration/epoch etc </param>
		Sub epochStart(ByVal sd As SameDiff, ByVal at As At)

		''' <summary>
		''' Called at the end of every epoch, when fitting from an iterator
		''' </summary>
		''' <param name="sd">              The SameDiff instance </param>
		''' <param name="at">              Current iteration/epoch etc </param>
		''' <param name="lossCurve">       The losses so far </param>
		''' <param name="epochTimeMillis"> How long this epoch took </param>
		''' <returns> ListenerResponse.STOP to stop training, CONTINUE or null to continue </returns>
		Function epochEnd(ByVal sd As SameDiff, ByVal at As At, ByVal lossCurve As LossCurve, ByVal epochTimeMillis As Long) As ListenerResponse

		''' <summary>
		''' Called after the end of every epoch, once validation evaluation is done, when training
		''' </summary>
		''' <param name="sd">                   The SameDiff instance </param>
		''' <param name="at">                   Current iteration/epoch etc </param>
		''' <param name="validationTimeMillis"> How long validation took for this epoch </param>
		''' <returns> ListenerResponse.STOP to stop training, CONTINUE or null to continue </returns>
		Function validationDone(ByVal sd As SameDiff, ByVal at As At, ByVal validationTimeMillis As Long) As ListenerResponse

		''' <summary>
		''' Called at the start of every iteration (minibatch), before any operations have been executed
		''' </summary>
		''' <param name="sd"> The SameDiff instance </param>
		''' <param name="at"> Current iteration/epoch etc </param>
		Sub iterationStart(ByVal sd As SameDiff, ByVal at As At, ByVal data As MultiDataSet, ByVal etlTimeMs As Long)

		''' <summary>
		''' Called at the end of every iteration, after all operations (including updating parameters) has been completed
		''' </summary>
		''' <param name="sd">      The SameDiff instance </param>
		''' <param name="at">      Current iteration/epoch etc </param>
		''' <param name="dataSet"> The current dataset (minibatch) used for training </param>
		''' <param name="loss">    The loss value for the current minibatch.  Will be null except for during training </param>
		Sub iterationDone(ByVal sd As SameDiff, ByVal at As At, ByVal dataSet As MultiDataSet, ByVal loss As Loss)

		''' <summary>
		''' Called at the start of an operation, e.g. training or validation
		''' </summary>
		''' <param name="sd"> The SameDiff instance </param>
		''' <param name="op"> The operation being started </param>
		Sub operationStart(ByVal sd As SameDiff, ByVal op As Operation)

		''' <summary>
		''' Called at the end of an operation, e.g. training or validation
		''' </summary>
		''' <param name="sd"> The SameDiff instance </param>
		''' <param name="op"> The operation being started </param>
		Sub operationEnd(ByVal sd As SameDiff, ByVal op As Operation)

		''' <summary>
		''' Called just before each operation is executed (native code called, etc) - after all inputs etc have been set
		''' </summary>
		''' <param name="sd"> The SameDiff instance </param>
		''' <param name="at"> Current iteration/epoch etc </param>
		''' <param name="op"> Operation that has just been executed </param>
		Sub preOpExecution(ByVal sd As SameDiff, ByVal at As At, ByVal op As SameDiffOp, ByVal opContext As OpContext)

		''' <summary>
		''' Called at the end of each operation execution<br>
		''' <para>
		''' Note: Outputs will most likely be freed later, use detach() if you need to save it.
		''' 
		''' </para>
		''' </summary>
		''' <param name="sd">      The SameDiff instance </param>
		''' <param name="at">      Current iteration/epoch etc </param>
		''' <param name="batch">   The batch's input data.  May be null if not called with a batch </param>
		''' <param name="op">      Operation that has just been executed </param>
		''' <param name="outputs"> The output arrays for the just-executed operation </param>
		Sub opExecution(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal opContext As OpContext, ByVal outputs() As INDArray)

		''' <summary>
		''' Called when any activation becomes available.
		''' <para>
		''' The activation will most likely be freed later, use dup() if you need to save it.<br>
		''' <br>
		''' Note that this method will be called when any activation becomes available, not just ones from <seealso cref="requiredVariables(SameDiff)"/><br>
		''' It is guaranteed to be called for variables from requiredVariables().<br>
		''' <br>
		''' Note that the activations here overlap with <seealso cref="opExecution(SameDiff, At, MultiDataSet, SameDiffOp, OpContext, INDArray[])"/> -
		''' both contain the same information/arrays
		''' 
		''' </para>
		''' </summary>
		''' <param name="sd">         The SameDiff instance </param>
		''' <param name="at">         Current iteration/epoch etc </param>
		''' <param name="batch">      The batch's input data.  May be null if not called with a batch </param>
		''' <param name="op">         Operation that has just been executed </param>
		''' <param name="varName">    The name of the variable </param>
		''' <param name="activation"> The variable's activation </param>
		Sub activationAvailable(ByVal sd As SameDiff, ByVal at As At, ByVal batch As MultiDataSet, ByVal op As SameDiffOp, ByVal varName As String, ByVal activation As INDArray)

		''' <summary>
		''' Called just before each parameter is to be updated - i.e., just before each parameter is modified.
		''' </summary>
		''' <param name="sd">     SameDiff instance </param>
		''' <param name="at">     The current iteration/epoch etc </param>
		''' <param name="v">      Variable about to be updated during backprop </param>
		''' <param name="update"> The array representing the update (i.e., the gradient after applying learning rate, momentum, etc) </param>
		Sub preUpdate(ByVal sd As SameDiff, ByVal at As At, ByVal v As Variable, ByVal update As INDArray)

	End Interface

End Namespace